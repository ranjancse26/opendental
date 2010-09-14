using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormExamSheets:Form {
		DataTable table;
		public long PatNum;
		private List <SheetDef> examSheets=null;

		public FormExamSheets() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormExamSheets_Load(object sender,EventArgs e) {
			Patient pat=Patients.GetLim(PatNum);
			Text=Lan.g(this,"Exam Sheets for")+" "+pat.GetNameFL();
			//fill show
			examSheets=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			for(int i=0;i<examSheets.Count;i++){
				comboExamType.Items.Add(examSheets[i].Description);
			}
			//select all rows in listShow
			FillGrid();
		}

		private void FillGrid(){
			//if a sheet is selected, remember it
			long selectedSheetNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				selectedSheetNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["SheetNum"].ToString());
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Time"),42);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Description"),210);
			gridMain.Columns.Add(col);
			gridMain.Rows.Clear();
			ODGridRow row;
			table=Sheets.GetExamSheetsTable(PatNum,DateTime.MinValue,DateTime.MaxValue,comboExamType.Text.ToString());
			for(int i=0;i<table.Rows.Count;i++){
				row=new ODGridRow();
				row.Cells.Add(table.Rows[i]["date"].ToString());
				row.Cells.Add(table.Rows[i]["time"].ToString());
				row.Cells.Add(table.Rows[i]["description"].ToString());
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
			if(selectedSheetNum!=0) {
				for(int i=0;i<table.Rows.Count;i++) {
					if(table.Rows[i]["SheetNum"].ToString()==selectedSheetNum.ToString()) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Sheets
			long sheetNum=PIn.Long(table.Rows[e.Row]["SheetNum"].ToString());
			Sheet sheet=Sheets.GetSheet(sheetNum);
			if(!Security.IsAuthorized(Permissions.SheetEdit,sheet.DateTimeSheet)){
				return;
			}
			FormSheetFillEdit FormSF=new FormSheetFillEdit(sheet);
			FormSF.ShowDialog();
			if(FormSF.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void menuItemSheets_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			FormSheetDefs FormSD=new FormSheetDefs();
			FormSD.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
			FillGrid();
		}

		private void comboExamType_SelectedValueChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefreshList_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SheetEdit)){//no date check, since no date for the sheet yet.
				return;
			}
			FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.ExamSheet;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;//only useful if not Terminal
			for(int i=0;i<FormS.SelectedSheetDefs.Count;i++) {
				sheetDef=FormS.SelectedSheetDefs[i];
				sheet=SheetUtil.CreateSheet(sheetDef,PatNum);
				SheetParameter.SetParameter(sheet,"PatNum",PatNum);
				SheetFiller.FillFields(sheet);
				SheetUtil.CalculateHeights(sheet,this.CreateGraphics());
			}
			FormSheetFillEdit FormSF=new FormSheetFillEdit(sheet);
			FormSF.ShowDialog();
			if(FormSF.DialogResult==DialogResult.OK) {
				FillGrid();
				gridMain.SetSelected(false);//unselect all rows
				gridMain.SetSelected(gridMain.Rows.Count-1,true);//Select the newly added row. Always last, since ordered by date.
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}
		

		

		
	}
}