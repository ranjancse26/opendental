using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans270Edit:Form {
		public Etrans EtransCur;
		private Etrans EtransAck271;
		private string MessageText;
		private string MessageTextAck;
		//public bool IsNew;//this makes no sense.  A 270 will never be new.  Always created, saved, and sent ahead of time.
		///<summary>True if the 270 and response have just been created and are being viewed for the first time.</summary>
		public bool IsInitialResponse;
		private List<EB271> listEB;
		private List<DTP271> listDTP;
		private X271 x271;
		public List<Benefit> benList;
		private int PatPlanNum;
		private int PlanNum;

		public FormEtrans270Edit(int patPlanNum,int planNum) {
			InitializeComponent();
			Lan.F(this);
			PatPlanNum=patPlanNum;
			PlanNum=planNum;
		}

		private void FormEtrans270Edit_Load(object sender,EventArgs e) {
			MessageText=EtransMessageTexts.GetMessageText(EtransCur.EtransMessageTextNum);
			MessageTextAck="";
			//textMessageText.Text=MessageText;
			textNote.Text=EtransCur.Note;
			EtransAck271=Etranss.GetEtrans(EtransCur.AckEtransNum);
			x271=null;
			if(EtransAck271!=null) {
				MessageTextAck=EtransMessageTexts.GetMessageText(EtransAck271.EtransMessageTextNum);//.Replace("~","~\r\n");
				if(EtransAck271.Etype==EtransType.BenefitResponse271) {
					x271=new X271(MessageTextAck);
				}
			}
			FillGridDates();
			FillGrid();
			FillGridBen();
			if(IsInitialResponse) {
				for(int i=0;i<listEB.Count;i++) {
					if(listEB[i].Benefitt !=null) {
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void FormEtrans270Edit_Shown(object sender,EventArgs e) {
			if(EtransAck271!=null && EtransAck271.Etype==EtransType.Acknowledge_997) {
				if(IsInitialResponse) {
					MessageBox.Show(EtransCur.Note);
				}
			}
		}

		private void FillGridDates() {
			listDTP=new List<DTP271>();
			if(x271 != null) {
				listDTP=x271.GetListDtpSubscriber();
			}
			gridDates.BeginUpdate();
			gridDates.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Date"),150);
			gridDates.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Qualifier"),230);
			gridDates.Columns.Add(col);
			gridDates.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<listDTP.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(listDTP[i].GetDateStr());
				row.Cells.Add(listDTP[i].GetQualifier());
				gridDates.Rows.Add(row);
			}
			gridDates.EndUpdate();
		}

		private void FillGrid(){
			//process the 271 to create a list of benefits--------------------------------------------------------
			listEB=new List<EB271>();
			if(x271 != null) {
				listEB=x271.GetListEB();
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Response"),420);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Import As Benefit"),420);
			gridMain.Columns.Add(col); 
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<listEB.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(listEB[i].GetDescription());
				if(listEB[i].Benefitt==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(listEB[i].Benefitt.ToString());
				}
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridBen() {
			gridBen.BeginUpdate();
			gridBen.Columns.Clear();
			ODGridColumn col=new ODGridColumn("",420);
			gridBen.Columns.Add(col);
			gridBen.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<benList.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(benList[i].ToString());
				gridBen.Rows.Add(row);
			}
			gridBen.EndUpdate();
		}

		private void gridBen_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int benefitListI=benList.IndexOf(benList[e.Row]);
			FormBenefitEdit FormB=new FormBenefitEdit(0,PlanNum);
			FormB.BenCur=benList[e.Row];
			FormB.ShowDialog();
			if(FormB.BenCur==null) {//user deleted
				benList.RemoveAt(benefitListI);
			}
			FillGridBen();
		}

		private void butImport_Click(object sender,EventArgs e) {
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				if(listEB[gridMain.SelectedIndices[i]].Benefitt==null){
					MsgBox.Show(this,"All selected rows must contain benefits to import.");
					return;
				}
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				benList.Add(listEB[gridMain.SelectedIndices[i]].Benefitt);
			}
			FillGridBen();
		}

		private void butShowRequest_Click(object sender,EventArgs e) {
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(MessageText);
			msgbox.ShowDialog();
		}

		private void butShowResponse_Click(object sender,EventArgs e) {
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(MessageTextAck);
			msgbox.ShowDialog();
		}

		/*
		private void butShowResponseDeciph_Click(object sender,EventArgs e) {
			if(!X12object.IsX12(MessageTextAck)) {
				MessageBox.Show("Only works with 997's");
				return;
			}
			X12object x12obj=new X12object(MessageTextAck);
			if(!x12obj.Is997()) {
				MessageBox.Show("Only works with 997's");
				return;
			}
			X997 x997=new X997(MessageTextAck);
			string display=x997.GetHumanReadable();
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(display);
			msgbox.ShowDialog();
		}*/

		private void butDelete_Click(object sender,EventArgs e) {
			//This button is not visible if IsNew
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire request and response?")) {
				return;
			}
			if(EtransAck271!=null) {
				EtransMessageTexts.Delete(EtransAck271.EtransMessageTextNum);
				Etranss.Delete(EtransAck271.EtransNum);
			}
			EtransMessageTexts.Delete(EtransCur.EtransMessageTextNum);
			Etranss.Delete(EtransCur.EtransNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			EtransCur.Note=textNote.Text;
			Etranss.Update(EtransCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			//if(IsNew) {
			//	EtransMessageTexts.Delete(EtransCur.EtransMessageTextNum);
			//	Etranss.Delete(EtransCur.EtransNum);
			//}
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		

		

	

		
	}
}