using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using OpenDental.DataAccess;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAnesthMedsEdit:Form {
		public AnesthMedsInventory Med;
		public List<AnesthMedInvAdjC> ListAnesthMedsInvAdj;
		public AnesthMedsInventoryAdj Adj;
		private AnesthMedsInventoryAdj AdjustNumCur;
        
		public FormAnesthMedsEdit() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormAnesthMedsEdit_Load(object sender,EventArgs e) {
			
			textAnesthMedName.Text = Med.AnesthMedName;
			textAnesthHowSupplied.Text = Med.AnesthHowSupplied;
			comboDEASchedule.Text = Med.DEASchedule;
			textQtyOnHand.Text = Med.QtyOnHand;
			//prevents editing of QtyOnHand from this form
			if (Med.QtyOnHand != "0")
			{

				textQtyOnHand.Enabled=false; 
			}
			if (Med.IsNew == true)
			{
				textQtyAdj.Enabled = false;
				textNotes.Enabled = false;
			}

		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(Med.IsNew){
				DialogResult=DialogResult.Cancel;
			}
			if(!MsgBox.Show(this,true,"Delete?")){
				return;
			}
			AnestheticMeds.DeleteObject(Med);
			DialogResult =DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			AdjustNumCur = new AnesthMedsInventoryAdj();

			if(textAnesthMedName.Text==""){
				MsgBox.Show(this,"Please enter a name.");
				return;
			}

			//write inventory data to anesthetic meds inventory db
			Med.AnesthMedName=textAnesthMedName.Text;
			Med.AnesthHowSupplied=textAnesthHowSupplied.Text;
			Med.DEASchedule = comboDEASchedule.Text;
			if (Med.IsNew != true)
			{
				Med.QtyOnHand = textQtyOnHand.Text;
			}
			if (Med.QtyOnHand == null){

				Med.QtyOnHand = "0";
			}

			if (Med.DEASchedule == null){

				Med.DEASchedule = "";
			}
			
			//write adjustment data to anesthetic meds inventory adjustment db
			Userod curUser = Security.CurUser;

			AdjustNumCur.AnestheticMedNum = Convert.ToInt32(Med.AnestheticMedNum);
			if (textQtyAdj.Text != "")
			{
				AdjustNumCur.QtyAdj = Convert.ToInt32(textQtyAdj.Text);
			}
			AdjustNumCur.UserNum = Convert.ToInt32(curUser.UserNum);
			if (textNotes.Text != "")
			{
				AdjustNumCur.Notes = textNotes.Text;
			}
			AdjustNumCur.TimeStamp = DateTime.Now;
		
			AnesthMedInvAdjs.Insert(AdjustNumCur);

			//write inventory adjustment back to table anesthmedsinventory
			int newQty = Convert.ToInt32(Med.QtyOnHand) + Convert.ToInt32(AdjustNumCur.QtyAdj);
			Med.QtyOnHand = newQty.ToString();

			AnestheticMeds.WriteObject(Med);
			DialogResult=DialogResult.OK;


		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void textAnesthMedName_TextChanged(object sender, EventArgs e){

		}

		private void textQtyOnHand_TextChanged(object sender, EventArgs e){
			
		}

		private void groupAnesthMedsEdit_Enter(object sender, EventArgs e){

		}

		private void textQtyAdj_TextChanged(object sender, EventArgs e)
		{

		}

	}
}