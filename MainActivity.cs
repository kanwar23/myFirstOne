using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Graphics;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Linq;
using Android.Util;
using Android.Graphics.Drawables;
using System.Net.Http;
using System.IO;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;


namespace DrawerLayoutTutorial
{
	[Activity (Label = "AppDemo", MainLauncher = true, Icon = "@drawable/Icon", Theme = "@style/CustomActionBarTheme")]

	public class MainActivity : Activity
	{
		DrawerLayout mDrawerLayout;
		List<string> mLeftItems = new List<string> ();

		int SetTag = 1;
		private ProgressBar progressBar;
		ActionBarDrawerToggle mDrawerToggle;
		public ListView Nebctext;
		public ArrayAdapter<string> azureAdap;
		List<string> newListForSpin = new List<string> ();

		//spinner
		private Spinner spinMasterPanel;
		private ArrayAdapter<string> spinnerArrayAdapter;
		List<string> First;
		//end spinner


		//Expandable List View Declarations
		private List<string> parentItems = new List<string> ();
		private List<object> childItems = new List<object> ();
		private Dictionary<string,List<string>> NewParentItems = new Dictionary<string,List<string>> ();
		private Dictionary<string,List<string>> dictPanelItemsForMail = new Dictionary<string, List<string>> ();

		public ExpandableListView mListExView;
		private LinearLayout leftDrawerLinearLayoutId;
		private myListViewAdap adapter;
		List<string> child = new List<string> ();
		List<string> childNewMail = new List<string> ();

		//End Expandable List View Declarations

		ListView tempListView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.myDrawer);
			progressBar = FindViewById<ProgressBar> (Resource.Id.loadingProgressBar);
			leftDrawerLinearLayoutId = (LinearLayout)FindViewById (Resource.Id.left_drawer);
			azureAdap = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1);
		
			//Start Expandable
			mListExView = FindViewById<ExpandableListView> (Resource.Id.expandListView);

			tempListView = FindViewById<ListView> (Resource.Id.lstViewTemp);
			//setMasterParent ();
			//setChild ();

			adapter = new myListViewAdap (NewParentItems);
			adapter.setInflater ((LayoutInflater)GetSystemService (Context.LayoutInflaterService), this);
			mListExView.SetAdapter (adapter);
			//End Expandable

			mDrawerToggle = new ActionBarDrawerToggle (this, mDrawerLayout, Resource.Drawable.ic_navigation_drawer, Resource.String.open_drawer, Resource.String.close_drawer);


			mDrawerLayout.SetDrawerListener (mDrawerToggle);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);
			ActionBar.SetDisplayShowTitleEnabled (true);
           
			//Declaration of Image button and other buttons
			ImageButton imgBtnPlus = (ImageButton)FindViewById (Resource.Id.buttonNewPanel);  
			Button btnCancel = (Button)FindViewById (Resource.Id.CancelPanel);
			Button btnSavePanel = (Button)FindViewById (Resource.Id.SavePanel);
			TableRow tRowSaveCancel = (TableRow)FindViewById (Resource.Id.tRowSaveCancel);    //Row of save Button
			EditText txtidofNewPanel = (EditText)(FindViewById (Resource.Id.txtOfNewPanel));
			spinMasterPanel = FindViewById<Spinner> (Resource.Id.spinnerExistingMasterPanel);


			//Code to add default value in the spinner
			First = new List<string> ();
			First.Add ("Select Parent Panel");
			spinnerArrayAdapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem, First);
			spinnerArrayAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinMasterPanel.Adapter = spinnerArrayAdapter;
			//End


			appendSpinner (spinMasterPanel, parentItems, spinnerArrayAdapter, First); //Calling spinner to append more parent.



			imgBtnPlus.Click += (object sender, EventArgs e) => {
				if (SetTag == 1) {
					imgBtnPlus.SetImageResource (Resource.Drawable.Multsign);
					txtidofNewPanel.Visibility = ViewStates.Visible;
					tRowSaveCancel.Visibility = ViewStates.Visible;
					spinMasterPanel.Visibility = ViewStates.Visible;
					SetTag = 0;

				} else if (SetTag == 0) {
					imgBtnPlus.SetImageResource (Resource.Drawable.plussign);

					txtidofNewPanel.Visibility = ViewStates.Gone;
					tRowSaveCancel.Visibility = ViewStates.Gone;
					spinMasterPanel.Visibility = ViewStates.Gone;
					SetTag = 1;
				}
			};

			btnCancel.Click += delegate {

				imgBtnPlus.SetImageResource (Resource.Drawable.plussign);
				tRowSaveCancel.Visibility = ViewStates.Gone;
				txtidofNewPanel.Visibility = ViewStates.Gone;
				spinMasterPanel.Visibility = ViewStates.Gone;

			};


			#region SavePanelCLickActivity

			btnSavePanel.Click += (object sender, EventArgs e) => {
				
				imgBtnPlus.SetImageResource (Resource.Drawable.plussign);
				tRowSaveCancel.Visibility = ViewStates.Gone;
				txtidofNewPanel.Visibility = ViewStates.Gone;
				spinMasterPanel.Visibility = ViewStates.Gone;
				string newPanelText = txtidofNewPanel.Text.Trim ();

				if (newPanelText != "") {
					string spinnerItem = spinMasterPanel.SelectedItem.ToString ();   // Selected item in the spinner

					if (spinnerItem != "Select Parent Panel") {
						child = new List<string> ();
						child.Add (newPanelText);

						if (NewParentItems.ContainsKey (spinnerItem)) {
							NewParentItems [spinnerItem].AddRange (child);
							dictPanelItemsForMail [spinnerItem].AddRange (child);
							
						} else {
                            
							NewParentItems [spinnerItem] = child;
							dictPanelItemsForMail [spinnerItem] = child;

						}


					} else {
						
						child = new List<string> ();
						NewParentItems [newPanelText] = child;
						dictPanelItemsForMail [newPanelText] = child;
						appendSpinner (spinMasterPanel, newListForSpin, spinnerArrayAdapter, First);
					}

					child = new List<string> ();
					childItems.Add (child);
					myListViewAdap adapterNew = new myListViewAdap (NewParentItems);
					adapterNew.setInflater ((LayoutInflater)GetSystemService (Context.LayoutInflaterService), this);
					mListExView.SetAdapter (adapter);									//Inflate Expandable Adapter
							
					txtidofNewPanel.Text = "";
					InputMethodManager inputmanager = (InputMethodManager)GetSystemService (Context.InputMethodService);
					inputmanager.HideSoftInputFromWindow (txtidofNewPanel.WindowToken, HideSoftInputFlags.None);		// To hide the keyboard

					spinMasterPanel.SetSelection (0, true);
				}
			};

			#endregion SavePanelActivityEnd


			// Code to hide keyboard on spinner touch
			spinMasterPanel.Touch += (object sender, View.TouchEventArgs e) => {

				InputMethodManager inputmanager = (InputMethodManager)GetSystemService (Context.InputMethodService);
				inputmanager.HideSoftInputFromWindow (spinMasterPanel.WindowToken, HideSoftInputFlags.None);	
				spinMasterPanel.PerformClick ();
			};


			// code to handle ChildClick Event
			mListExView.ChildClick += mListExView_ChildClick;

	
		}

		MobileServiceClient client1;
		IMobileServiceSyncTable<OutlookMailItem> outlookMasterStoreTable;
		public List<string> mailItems;


		void mListExView_ChildClick (object sender, ExpandableListView.ChildClickEventArgs e)
		{
			azureAdap.Clear ();
			tempListView.Adapter = azureAdap;
			progressBar.Visibility = ViewStates.Visible;
			mDrawerLayout.CloseDrawer (leftDrawerLinearLayoutId);
			string dictItem = dictPanelItemsForMail [dictPanelItemsForMail.Keys.ElementAt (e.GroupPosition).ToString ()].ElementAt (e.ChildPosition).ToString ();
			int indexofColon = dictItem.IndexOf (':');

			string idOfPanel = dictItem.Substring (indexofColon + 1, (dictItem.Length - indexofColon) - 1);

			mailItemsCall (idOfPanel);
			
		}

		public async void mailItemsCall (string idOfPanel)
		{
			try {


				await fetchMailItems (idOfPanel);
			} catch {

				CreateAndShowDialog ("Cannot connect to internet", "Connection Problem");
				progressBar.Visibility = ViewStates.Gone;
			
			}
		}


		async Task fetchMailItems (string idOfPanel)
		{

			try {
				

				mailItems = await outlookMasterStoreTable.Where (O => O.SearchMasterItemId == idOfPanel)
					.Select (o => o.SenderName + ':' + o.Subject + ':' + o.To).ToListAsync ();

				if (mailItems.Count == 0) {
					CreateAndShowDialog ("No Records Found", "Choose another option");
					progressBar.Visibility = ViewStates.Gone;
				} else {
					azureAdap.AddAll (mailItems);
					tempListView.Adapter = azureAdap;
				}
			} catch {
				CreateAndShowDialog ("ERROR IN CONNECTION", "Connection Problem");
				progressBar.Visibility = ViewStates.Gone;
			}

			progressBar.Visibility = ViewStates.Gone;
		}


//		public async void mailItemsCall (string idOfPanel)
//		{
//			try {
//				client1 = new MobileServiceClient ("https://testmobileservice-pb.azure-mobile.net/", "gvMFfuXlwQexmTVMhvcvywmbbzCBnV82");
//				string path = System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "local.db");
//
//				if (!File.Exists (path)) {
//					File.Create (path).Dispose ();
//				}
//
//
//				var store = new MobileServiceSQLiteStore (path);
//				store.DefineTable<OutlookMailItem> ();
//
//				await client1.SyncContext.InitializeAsync (store);
//
//				outlookMasterStoreTable = client1.GetSyncTable<OutlookMailItem> ();
//
//
//				await fetchMailItems (idOfPanel);
//			} catch {
//
//				CreateAndShowDialog ("Cannot connect to internet", "Connection Problem");
//				progressBar.Visibility = ViewStates.Gone;
//
//			}
//		}
//
//
//		async Task fetchMailItems (string idOfPanel)
//		{
//
//			try {
//				await client1.SyncContext.PushAsync ();
//				await outlookMasterStoreTable.PullAsync (null, outlookMasterStoreTable.CreateQuery ());
//
//				mailItems = await outlookMasterStoreTable.Where (O => O.SearchMasterItemId == idOfPanel)
//					.Select (o => o.SenderName + ':' + o.Subject + ':' + o.To).ToListAsync ();
//
//				if (mailItems.Count == 0) {
//					CreateAndShowDialog ("No Records Found", "Choose another option");
//					progressBar.Visibility = ViewStates.Gone;
//				} else {
//					azureAdap.AddAll (mailItems);
//					tempListView.Adapter = azureAdap;
//				}
//			} catch {
//				CreateAndShowDialog ("ERROR IN CONNECTION", "Connection Problem");
//				progressBar.Visibility = ViewStates.Gone;
//			}
//
//			progressBar.Visibility = ViewStates.Gone;
//		}
//


		#region code for listViewAzure

		//Code for ListViewAzure
		MobileServiceClient client;


		IMobileServiceSyncTable<PanelItem> panelItemStoreTable;
		IMobileServiceSyncTable<PanelMasterItem> panelMasterItemStoreTable;

		public List<string> listofMasterPanel;
		public List<string> listofPanelItem;
		private string path;
		int lenPanelItemTillName, lenPanelItemTillId, lenMasterItem;
		string newPanelItem;




		public async void connectMaster ()
		{
			try {
				client = new MobileServiceClient ("https://testmobileservice-pb.azure-mobile.net/", "gvMFfuXlwQexmTVMhvcvywmbbzCBnV82");

				path = System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "local.db");

				if (!File.Exists (path)) {
					File.Create (path).Dispose ();
				}


				var store = new MobileServiceSQLiteStore (path);
				store.DefineTable<PanelItem> ();
				store.DefineTable<PanelMasterItem> ();
				store.DefineTable<OutlookMailItem> ();
				await client.SyncContext.InitializeAsync (store);

				panelItemStoreTable = client.GetSyncTable<PanelItem> ();
				panelMasterItemStoreTable = client.GetSyncTable<PanelMasterItem> ();
				outlookMasterStoreTable = client.GetSyncTable<OutlookMailItem>();
				await SyncAsync ();


			} catch {
				CreateAndShowDialog ("Cannot connect to internet", "Connection Problem");
				progressBar.Visibility = ViewStates.Gone;
			}
		}

		async Task  SyncAsync ()
		{
			try {
		
				await client.SyncContext.PushAsync ();
				await panelItemStoreTable.PullAsync (null, panelItemStoreTable.CreateQuery ());
				await panelMasterItemStoreTable.PullAsync (null, panelMasterItemStoreTable.CreateQuery ());
				await outlookMasterStoreTable.PullAsync (null, outlookMasterStoreTable.CreateQuery ());

				listofMasterPanel = await panelMasterItemStoreTable.Select (j => j.Id + ',' + j.PanelName).ToListAsync ();

				listofPanelItem = await panelItemStoreTable.OrderByDescending (j => j.MasterPanelID)
					.Select (i => i.MasterPanelID + ',' + i.PanelName + ':' + i.Id).ToListAsync ();

				NewParentItems.Clear ();
				dictPanelItemsForMail.Clear ();


				foreach (string panelItem in listofPanelItem) {

					lenPanelItemTillName = panelItem.IndexOf (',');
					lenPanelItemTillId = panelItem.IndexOf (':');

					string newItem = panelItem.Substring (0, lenPanelItemTillName);
					newPanelItem = panelItem.Substring (lenPanelItemTillName + 1, (lenPanelItemTillId - lenPanelItemTillName) - 1);

					foreach (string masteritem in listofMasterPanel) {

						if (lenPanelItemTillName == 0) {
							child = new List<string> ();
							NewParentItems [newPanelItem] = child;
							dictPanelItemsForMail [newPanelItem] = child;
							break;

						}

						lenMasterItem = masteritem.IndexOf (',');
						string newMasterItem = masteritem.Substring (0, lenMasterItem);

						if (newItem.Equals (newMasterItem)) {
							newMasterItem = masteritem.Substring (lenMasterItem + 1, (masteritem.Length - lenMasterItem) - 1);
							child = new List<string> ();
							childNewMail = new List<string> ();
							childNewMail.Add (panelItem);
							child.Add (newPanelItem); 

							if (NewParentItems.ContainsKey (newMasterItem)) {

								NewParentItems [newMasterItem].AddRange (child);
								dictPanelItemsForMail [newMasterItem].AddRange (childNewMail);

								newListForSpin.Add (newMasterItem);
								appendSpinner (spinMasterPanel, newListForSpin, spinnerArrayAdapter, First);

							} else {
								newListForSpin.Add (newMasterItem);
								NewParentItems [newMasterItem] = child;
								dictPanelItemsForMail [newMasterItem] = childNewMail;
								appendSpinner (spinMasterPanel, newListForSpin, spinnerArrayAdapter, First);

							}
						}

					}
				}
				adapter = new myListViewAdap (NewParentItems);
				adapter.setInflater ((LayoutInflater)GetSystemService (Context.LayoutInflaterService), this);
				progressBar.Visibility = ViewStates.Gone;
				mListExView.SetAdapter (adapter);



			} catch (MobileServiceInvalidOperationException e) {


				Console.WriteLine (e);
			}

			mDrawerLayout.OpenDrawer (leftDrawerLinearLayoutId);
		}


		void CreateAndShowDialog (string message, string title)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder (this);

			builder.SetMessage (message);
			builder.SetTitle (title);
			builder.Create ().Show ();
		}

		#endregion


		public void  appendSpinner (Spinner spinMasterPanel, List<string> spinnerParentItems, ArrayAdapter<string> spinnerArrayAdapter, List<string> First)	//Appending more items in the spinner
		{
			spinnerArrayAdapter.Clear ();
			spinnerArrayAdapter.NotifyDataSetChanged ();
			spinnerArrayAdapter.AddAll (First);
			spinnerArrayAdapter.AddAll (spinnerParentItems);
			spinnerArrayAdapter.NotifyDataSetChanged ();
		}

	

		public void setChild ()				//Child
		{
			child.Add ("Sms");
			child.Add ("RandomChild");
			NewParentItems.Add ("MasterPanel1", child);
			dictPanelItemsForMail.Add ("MasterPanel1", child);
			child = new List<string> ();
			child.Add ("Str");
			child.Add ("Ronakt");
			NewParentItems ["MasterPanel2"] = child;
			dictPanelItemsForMail.Add ("MasterPanel2", child);
			child = new List<string> ();
			NewParentItems ["Rest"] = child;
			dictPanelItemsForMail ["Rest"] = child;


		}

    

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState ();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (mDrawerToggle.OnOptionsItemSelected (item)) {
			
				return true;
			}

			switch (item.ItemId) {
			case Resource.Id.syncAzureData:
				progressBar.Visibility = ViewStates.Visible;
                //connectMasterStore();
				connectMaster ();

				return true;

			}

			return base.OnOptionsItemSelected (item);
		}



		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged (newConfig);
		}

		public override bool OnCreateOptionsMenu (IMenu Menu)
		{
			MenuInflater.Inflate (Resource.Menu.action_bar, Menu);
			return base.OnCreateOptionsMenu (Menu);

		}
	}

	


}


