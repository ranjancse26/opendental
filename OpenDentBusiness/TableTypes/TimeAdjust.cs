using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Used on employee timecards to make adjustments.</summary>
	[Serializable]
	public class TimeAdjust:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TimeAdjustNum;
		///<summary>FK to employee.EmployeeNum</summary>
		public long EmployeeNum;
		///<summary>The date and time that this entry will show on timecard.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime TimeEntry;
		///<summary>The number of regular hours to adjust timecard by.  Can be + or -.</summary>
		public TimeSpan RegHours;
		///<summary>Overtime hours. Usually +.  Automatically combined with a - adj to RegHours.</summary>
		public TimeSpan OTimeHours;
		///<summary>.</summary>
		public string Note;
		
		///<summary></summary>
		public TimeAdjust Copy() {
			return (TimeAdjust)MemberwiseClone();
		}


		




	}

	
}




