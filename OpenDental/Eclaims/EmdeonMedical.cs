using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Ionic.Zip;

namespace OpenDental.Eclaims
{
	public class EmdeonMedical{

		///<summary></summary>
		public EmdeonMedical()
		{			
		}

		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created X12 file. The batchnum is supplied for the possible rollback.  Also used for mail retrieval.</summary>
		public static bool Launch(Clearinghouse clearhouse,int batchNum,EnumClaimMedType medType){
			EmdeonITSTest.ITSWS itsws=new EmdeonITSTest.ITSWS();
			itsws.Url="https://cert.its.emdeon.com/ITS/ITSWS.asmx";//test url
			//itsws.Url="https://its.emdeon.com/ITS/ITSWS.asmx";//production url
			string messageType="";
			if(batchNum!=0) { //batchNum will not be zero when a batch is being sent but would be zero when getting reports.
				string batchFile="";
				Etrans etrans=null;
				try {
					if(!Directory.Exists(clearhouse.ExportPath)) {
						throw new Exception("Clearinghouse export path is invalid.");
					}
					string[] files=Directory.GetFiles(clearhouse.ExportPath);
					if(files.Length>0) {
						//We make sure to only send the X12 batch file for the current batch, so that if there is a failure, then we know
						//for sure that we need to reverse the batch. This will also help us avoid any exterraneous/old batch files in the
						//same directory which might be left if there is a permission issue when trying to delete the batch files after processing.
						for(int i=0;i<files.Length;i++) {
							if(Path.GetFileName(files[i])=="claims"+batchNum+".txt") {
								batchFile=files[i];
								break;
							}
						}
						byte[] fileBytes=File.ReadAllBytes(batchFile);
						string fileBytesBase64=Convert.ToBase64String(fileBytes);
						messageType="MCT";//medical
						if(medType==EnumClaimMedType.Institutional) {
							messageType="HCT";
						}
						else if(medType==EnumClaimMedType.Dental) {
							messageType="DCT";//not used/tested yet, but planned for future.
						}
						EmdeonITSTest.ITSReturn response=itsws.PutFileExt(clearhouse.LoginID,clearhouse.Password,messageType,Path.GetFileName(batchFile),fileBytesBase64);
						etrans=new Etrans();
						etrans.BatchNumber=batchNum;
						etrans.ClearingHouseNum=clearhouse.ClearinghouseNum;
						etrans.DateTimeTrans=DateTime.Now;
						etrans.Etype=EtransType.TextReport;
						etrans.Note=response.Response;
						if(response.ErrorCode==0) { //Batch submission successful.
							etrans.AckCode="A";
							Etranss.Insert(etrans);
						}
						else { //Batch submission rejection/error.
							etrans.AckCode="R";
							Etranss.Insert(etrans);
							throw new Exception("Emdeon rejected all claims in the current batch file "+batchFile+" for the following reason: "+response.Response);
						}
					}
				}
				catch(Exception e) {
					MessageBox.Show(e.Message);
					x837Controller.Rollback(clearhouse,batchNum);
					return false;
				}
				finally {
					if(batchFile!="") {
						try {
							File.Delete(batchFile);
						}
						catch {
							MessageBox.Show("Failed to remove batch file "+batchFile+". This is probably due to a permission issue. Check folder permissions and delete this file manually.");
						}
					}
				}
			}
			else {//batchNum=0, therefore getting reports.
				if(!Directory.Exists(clearhouse.ResponsePath)) {
					throw new Exception("Clearinghouse response path is invalid.");
				}
				try {
					//Download the most up to date reports, but do not delete them from the server yet.
					messageType="MCTG";//medical
					if(medType==EnumClaimMedType.Institutional) {
						messageType="HCTG";
					}
					else if(medType==EnumClaimMedType.Dental) {
						messageType="DCTG";
					}
					EmdeonITSTest.ITSReturn response=itsws.GetFile(clearhouse.LoginID,clearhouse.Password,messageType);
					if(response.ErrorCode==0) { //Report retrieval successful.
						string reportFileDataBase64=response.Response;
						byte[] reportFileDataBytes=Convert.FromBase64String(reportFileDataBase64);
						string reportFilePath=CodeBase.ODFileUtils.CreateRandomFile(clearhouse.ResponsePath,".zip");
						File.WriteAllBytes(reportFilePath,reportFileDataBytes);
						//Now that the file has been saved, remove the report file from the Emdeon server.
						messageType="MCTD";//medical
						if(medType==EnumClaimMedType.Institutional) {
							messageType="HCTD";
						}
						else if(medType==EnumClaimMedType.Dental) {
							messageType="DCTD";
						}
						//If deleting the report fails, we don't care because that will simply mean that we download it again next time.
						//Thus we don't need to check the status after this next call.
						itsws.GetFile(clearhouse.LoginID,clearhouse.Password,messageType);
					}
					else { //Report retrieval failure.
						throw new Exception("Failed to get reports from Emdeon. Error message from Emdeon: "+response.Response);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return false;
				}
			}
			return true;
		}

	}
}