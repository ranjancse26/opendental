using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReferenceEntryEdit:Form {
		private CustRefEntry CustRefEntryCur;

		public FormReferenceEntryEdit(CustRefEntry refEntry) {
			InitializeComponent();
			Lan.F(this);
			CustRefEntryCur=refEntry;
		}

		private void FormReferenceEdit_Load(object sender,EventArgs e) {
			Patient patCust=Patients.GetLim(CustRefEntryCur.PatNumCust);
			Patient patRef=Patients.GetLim(CustRefEntryCur.PatNumRef);
			textCustomer.Text=Patients.GetNameFL(patCust.LName,patCust.FName,patCust.Preferred,patCust.MiddleI);
			textReferredTo.Text=Patients.GetNameFL(patRef.LName,patRef.FName,patRef.Preferred,patRef.MiddleI);
			if(CustRefEntryCur.DateEntry.Year>1880) {
				textDateEntry.Text=CustRefEntryCur.DateEntry.ToShortDateString();
			}
			textNote.Text=CustRefEntryCur.Note;
		}

		private void butDeleteAll_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete Reference Entry?")) {
				return;
			}
			CustRefEntries.Delete(CustRefEntryCur.CustRefEntryNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			CustRefEntryCur.Note=textNote.Text;
			CustRefEntries.Update(CustRefEntryCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}