using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class ScheduleOps {
		///<summary></summary>
		public static void Insert(ScheduleOp op){
			if(PrefC.RandomKeys){
				op.ScheduleOpNum=MiscData.GetKey("scheduleop","ScheduleOpNum");
			}
			string command= "INSERT INTO scheduleop (";
			if(PrefC.RandomKeys){
				command+="ScheduleOpNum,";
			}
			command+="ScheduleNum,OperatoryNum) VALUES(";
			if(PrefC.RandomKeys){
				command+="'"+POut.PInt(op.ScheduleOpNum)+"', ";
			}
			command+=
				 "'"+POut.PInt   (op.ScheduleNum)+"', "
				+"'"+POut.PInt   (op.OperatoryNum)+"')";
			if(PrefC.RandomKeys) {
				General.NonQ(command);
			}
			else {
				op.ScheduleOpNum=General.NonQ(command,true);
			}
		}

		
	}

	

	

}













