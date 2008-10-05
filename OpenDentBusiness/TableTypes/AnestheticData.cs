﻿using System;

namespace OpenDentBusiness
{

	///<summary>Anesthetic data from an Anesthetic Record.</summary>
	public class AnestheticData{
		///<summary>Primary key.</summary>
        public int AnestheticDataNum;
        ///<summary>FK to anestheticRecord.AnestheticRecordNum.</summary>
		public int AnestheticRecordNum;
		public DateTime AnesthOpen;
		public DateTime AnesthClose;
		public DateTime SurgOpen;
		public DateTime SurgClose;
		public string Anesthetist; //data from OD provider list
		public string Surgeon; //data from OD provider list
		public string Asst; //data from OD provider list
		public string Circulator; //data from OD provider list
		public string VSMName;
		public string VSMSerNum; 
		public string AnesthMed;
		public float AnesthDose;
		public DateTime DoseTimeStamp;
		public string ASA;
		public char ASA_EModifier;
		public bool InhO2;
		public bool InhN20;
		public int O2Lmin;
		public int N20Lmin;
		public bool RteNasCan;
		public bool RteNasHood;
		public bool RteETT;
		public bool MedRouteIVCath;
		public bool MedRouteIVButFly;
		public bool MedRouteIM;
		public bool MedRoutePO;
		public bool MedRouteNasal;
		public bool MedRouteRectal;
		public string IVSite;
		public int IVGauge;
		public bool IVSiteR;
		public bool IVSiteL;
		public int IVAtt;
		public string IVF;
		public int IVFVol;
		public string Notes;
		public int PatWgt;
		public bool WgtUnitLbs;
		public bool WgtUnitKgs;
		public string PatHgt;
		public string EscortName;
		public string EscortCellNum;
		public string EscortRel;
		public string NPOTime;
		public bool SigIsTopaz;
		public string SigBox;

		
	}





}

