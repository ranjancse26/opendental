using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrProvKeysCustomer:Form {
		private List<EhrProvKey> listKeys;
		public long Guarantor;

		public FormEhrProvKeysCustomer() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormEhrProvKeysCustomer_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"LName"),100);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"FName"),100);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Key"),100);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"ProcFee"),60,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"FTE"),35,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Notes"),100);
			gridMain.Columns.Add(col);
			//todo: add more columns	
			listKeys=EhrProvKeys.RefreshForFam(Guarantor);
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<listKeys.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(listKeys[i].LName);
				row.Cells.Add(listKeys[i].FName);
				row.Cells.Add(listKeys[i].ProvKey);
				if(listKeys[i].ProcNum==0) {
					row.Cells.Add("");
				}
				else {
					Procedure proc=Procedures.GetOneProc(listKeys[i].ProcNum,false);
					row.Cells.Add(proc.ProcFee.ToString("c"));
				}
				row.Cells.Add(listKeys[i].FullTimeEquiv.ToString());
				row.Cells.Add(listKeys[i].Notes);
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FormEhrProvKeyEditCust formK=new FormEhrProvKeyEditCust();
			formK.KeyCur=listKeys[e.Row];
			formK.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FormEhrProvKeyEditCust formK=new FormEhrProvKeyEditCust();
			formK.KeyCur=new EhrProvKey();
			formK.KeyCur.PatNum=Guarantor;
			formK.KeyCur.FullTimeEquiv=1;
			formK.KeyCur.IsNew=true;
			formK.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	

		

		
	}
}