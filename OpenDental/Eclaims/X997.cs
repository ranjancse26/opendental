using System;

namespace OpenDental.Eclaims
{
	///<summary></summary>
	public class X997{
		public bool Is997(string message){
			string[] lines=message.Split('~');
			if(lines.Length>0 && lines[0]!=null && lines[0].Length==106 && lines[0].Substring(0,3)=="ISA"){
				return true;
			}
			return false;
					//try{
					//	string humanText=X277U.MakeHumanReadable((string)listMain.SelectedItem);
		}

		




	}
}
