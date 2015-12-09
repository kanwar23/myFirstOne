using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using System.Drawing;

namespace DrawerLayoutTutorial
{
	class myListViewAdap :BaseExpandableListAdapter
	{
		private Activity activity;
		//private List<object> childtems;
		private LayoutInflater inflater;
		//private List<string> parentItems, child;
        private List<string>  child;
        Dictionary<string, List<string>> parentChildrenDict;
		//  private  int listGroupCount=-1;
		//	MainActivity cs;
        //public myListViewAdap(List<string> parents,List<object> children)
        //{
        //    this.parentItems = parents;
        //    this.childtems = children;
        //}

        public myListViewAdap(Dictionary<string,List<string> > dicttion)
        {
            this.parentChildrenDict = dicttion;

        }
		public void setInflater(LayoutInflater inflater, Activity activity)
		{
			this.inflater = inflater;
			this.activity = activity;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
            
			child = (List<string>)parentChildrenDict [parentChildrenDict.Keys.ElementAt(groupPosition).ToString()];

		
            TextView textview = null;

			if (convertView == null) {
				convertView = inflater.Inflate (Resource.Layout.group, null);
			}


			textview = (TextView)convertView.FindViewById (Resource.Id.textViewgroup1);
			textview.Text = (child[childPosition]);

		
		
			return convertView;
		}

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			if (convertView == null){

				convertView = inflater.Inflate (Resource.Layout.row, null);

			}

			if (GetChildrenCount (groupPosition) != 0) {
				if (isExpanded) {

                    ((TextView)convertView).RequestFocus();
           
					((TextView)convertView).SetCompoundDrawablesWithIntrinsicBounds (0, 0, Resource.Drawable.minusExpandableList, 0);

				} else {
					((TextView)convertView).SetCompoundDrawablesWithIntrinsicBounds (0, 0, Resource.Drawable.plusExandableList, 0);
			
				}
               

			}
            else
            {
                ((TextView)convertView).SetCompoundDrawablesWithIntrinsicBounds(0, 0,0, 0);
            }

          
            ((CheckedTextView)convertView).Text = (parentChildrenDict.Keys.ElementAt(groupPosition));
			return convertView;
		}

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return 0;
		}

		public override int GetChildrenCount (int groupPosition)
		{
            int C = ((List<string>)parentChildrenDict[parentChildrenDict.Keys.ElementAt(groupPosition).ToString()]).Count();

			return C;
		}


		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
		}


		public override int GroupCount {
			get {
				//return parentItems.Count ();
                return parentChildrenDict.Keys.Count();
			}
		}

		public override void OnGroupCollapsed (int groupPosition)
		{
			base.OnGroupCollapsed (groupPosition);
		}

		public override void OnGroupExpanded (int groupPosition)
		{
//			if (listGroupCount == -1)
//				listGroupCount= groupPosition;
//			else {
//				cs.mListExView.CollapseGroup (listGroupCount);
//			}
			base.OnGroupExpanded (groupPosition);
			//listGroupCount = groupPosition;
		}

		public override long GetGroupId (int groupPosition)
		{
			return 0;
		}

		public override bool HasStableIds {
			get {
				return false;
			}
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}


	


	}


}

