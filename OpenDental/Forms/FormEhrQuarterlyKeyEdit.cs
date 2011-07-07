using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrQuarterlyKeyEdit:Form {
		public EhrQuarterlyKey KeyCur;

		public FormEhrQuarterlyKeyEdit() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormEhrQuarterlyKeyEdit_Load(object sender,EventArgs e) {
			if(KeyCur.YearValue>0) {
				textYear.Text=KeyCur.YearValue.ToString();
			}
			if(KeyCur.QuarterValue>0) {
				textQuarter.Text=KeyCur.QuarterValue.ToString();
			}
			textKey.Text=KeyCur.KeyValue;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(KeyCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			EhrQuarterlyKeys.Delete(KeyCur.EhrQuarterlyKeyNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textYear.errorProvider1.GetError(textYear)!=""
				|| textQuarter.errorProvider1.GetError(textQuarter)!="") 
			{
				MessageBox.Show("Please fix errors first.");
				return;
			}
			KeyCur.YearValue=PIn.Int(textYear.Text);
			KeyCur.QuarterValue=PIn.Int(textQuarter.Text);
			KeyCur.KeyValue=textKey.Text;
			if(KeyCur.IsNew) {
				EhrQuarterlyKeys.Insert(KeyCur);
			}
			else {
				EhrQuarterlyKeys.Update(KeyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}