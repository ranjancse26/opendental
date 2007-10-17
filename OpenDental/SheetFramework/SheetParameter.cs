using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental{
	///<Summary></Summary>
	class SheetParameter {
		///<Summary></Summary>
		public bool IsRequired;
		///<Summary>Usually, a columnName.</Summary>
		public string ParamName;
		///<Summary>This is the value which must be set in order to obtain data from the database. It is usually an int primary key.</Summary>
		public object ParamValue;

		public SheetParameter(bool isRequired,string paramName) {
			IsRequired=isRequired;
			ParamName=paramName;
		}

		///<Summary>Every sheet has at least one required parameter, usually the primary key of an imporant table.</Summary>
		public static List<SheetParameter> GetForType(SheetTypeEnum sheetType) {
			List<SheetParameter> list=new List<SheetParameter>();
			if(sheetType==SheetTypeEnum.LabelPatient) {
				list.Add(new SheetParameter(true,"PatNum"));
			}
			if(sheetType==SheetTypeEnum.LabelCarrier) {
				list.Add(new SheetParameter(true,"CarrierNum"));
			}
			if(sheetType==SheetTypeEnum.LabelReferral) {
				list.Add(new SheetParameter(true,"ReferralNum"));
			}
			return list;
		}

	}

}
