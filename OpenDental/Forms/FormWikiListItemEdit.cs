using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormWikiListItemEdit:Form {
		///<summary>Name of the wiki list.</summary>
		public string WikiListCur;
		public long ItemNum;
		public bool IsNew;
		///<summary>Creating a data table containing only one item allows us to use column names.</summary>
		DataTable ItemTable;

		public FormWikiListItemEdit() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormWikiListEdit_Load(object sender,EventArgs e) {
			ItemTable = WikiLists.GetItem(WikiListCur,ItemNum);
			FillGrid();
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Column"),200);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Value"),400);
			col.IsEditable=true;
			gridMain.Columns.Add(col);
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<ItemTable.Columns.Count;i++){
				row=new ODGridRow();
				row.Cells.Add(ItemTable.Columns[i].ColumnName);
				row.Cells.Add(ItemTable.Rows[0][i].ToString());
				if(i==0) {
					row.ColorBackG=Color.Gray;
				}
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.Title="Edit List Item";
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			//new big window to edit row
		}

		private void gridMain_CellTextChanged(object sender,EventArgs e) {
			
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			//update all cells. No call to DB, so this should be safe.
			for(int i=1;i<gridMain.Rows.Count;i++) {//start at one, because we should never change the PK.
				ItemTable.Rows[0][i]=gridMain.Rows[i].Cells[1].Text;
			}
			//not entirely neccesary, but will "undo" changes made to PK.
			FillGrid();
		}

		/*No longer necessary because gridMain_CellLeave does this as text is changed.
		///<summary>This is done before generating markup, when adding or removing rows or columns, and when changing from "none" view to another view.  FillGrid can't be done until this is done.</summary>
		private void PumpGridIntoTable() {
			//table and grid will only have the same numbers of rows and columns if the view is none.
			//Otherwise, table may have more columns
			//So this is only allowed when switching from the none view to some other view.
			if(ViewShowing!=0) {
				return;
			}
			for(int i=0;i<Table.Rows.Count;i++) {
				for(int c=0;c<Table.Columns.Count;c++) {
					Table.Rows[i][c]=gridMain.Rows[i].Cells[c].Text;
				}
			}
		}*/

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this list item and all references to it?")) {
				return;
			}
			WikiLists.DeleteItem(WikiListCur,ItemNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			WikiLists.UpdateItem(WikiListCur,ItemTable);
			DialogResult=DialogResult.OK;
			//TODO:refresh list data from the form that called this one.
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		
	

	

		

		

		

	

	}
}