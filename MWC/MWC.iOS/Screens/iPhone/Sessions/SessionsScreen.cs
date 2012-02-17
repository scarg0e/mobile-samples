using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MWC.iOS.Screens.iPad.Sessions;

namespace MWC.iOS.Screens.iPhone.Sessions {
	/// <summary>
	/// Speakers screen. Derives from MonoTouch.Dialog's DialogViewController to do 
	/// the heavy lifting for table population.
	/// </summary>
	public partial class SessionsScreen : UpdateManagerLoadingDialogViewController {
		public IList<BL.Session> Sessions;
		
		/// <summary>If this is null, on iPhone; otherwise on iPad</summary>
		SessionSplitView splitView;

		/// <summary>for iPhone</summary>
		public SessionsScreen () : this (null)
		{
		}
		
		/// <summary>for iPad</summary>
		public SessionsScreen (SessionSplitView sessionSplitView) : base ()
		{
			splitView = sessionSplitView;
			EnableSearch = true; // requires SessionElement to implement Matches()
		}

		/// <summary>
		/// Populates the page with sessions, grouped by time slot
		/// </summary>
		protected override void PopulateTable()
		{
			Sessions = BL.Managers.SessionManager.GetSessions ();
			
			Root = 	new RootElement ("Sessions") {
					from session in Sessions
						group session by session.Start.Ticks into timeslot
						orderby timeslot.Key
						select new Section (new DateTime (timeslot.Key).ToString("dddd HH:mm") ) {
						from eachSession in timeslot
						   select (Element) new MWC.iOS.UI.CustomElements.SessionElement (eachSession, splitView)
			}};
			// hide search until pull-down
			TableView.ScrollToRow (NSIndexPath.FromRowSection (0,0), UITableViewScrollPosition.Top, false);
		}	
		
		// scroll back to the point where you last were in the list
		NSIndexPath lastScrollY;
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			lastScrollY = TableView.IndexPathForSelectedRow;
			
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (lastScrollY != null)
				TableView.ScrollToRow (lastScrollY, UITableViewScrollPosition.Middle, false);
		}
	}
}