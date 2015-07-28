﻿using System;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Diagnostics;
using ROMPCheckIn.cms.romponline.com;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Gms.Location;
using Android.Util;
using Android.Gms.Common.Apis;
using Android.Gms.Common;

namespace ROMPCheckIn
{
	[Activity (Label = "ROMP Check-In", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.NoTitle);
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.btnLogin);
			button.Click += delegate {
				TextView txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);
				TextView txtUsername = FindViewById<TextView> (Resource.Id.txtUsername);
				string email = txtUsername.Text;
				Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
				Match match = regex.Match(email);
				if (string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtUsername.Text)) {
					var myHandler = new Handler();
					myHandler.Post(() => {
						Android.Widget.Toast.MakeText(this, "Please Provide a Username and Password.", Android.Widget.ToastLength.Long).Show();
					});
				} else if (!match.Success){
					var myHandler = new Handler();
					myHandler.Post(() => {
						Android.Widget.Toast.MakeText(this, "Not a valid Email Address.", Android.Widget.ToastLength.Long).Show();
					});
				} else {
					try {						
						var locSvc = new ROMPLocation();
						var loginResp = new LoginResponse();
						loginResp = locSvc.LearnerLogin(txtUsername.Text, txtPassword.Text);
						if (loginResp.Success) {
							if (loginResp.GroupID <= 2) {
								var nextActivity = new Intent(this, typeof(ChooseModeActivity));
								nextActivity.PutExtra("SessionKey", loginResp.SessionKey);
								nextActivity.PutExtra("GroupID", loginResp.GroupID);
								nextActivity.PutExtra("UserID", loginResp.UserID);
								StartActivity(nextActivity);
								Finish();
							} else {
								var nextActivity = new Intent(this, typeof(CheckInActivity));
								nextActivity.PutExtra("SessionKey", loginResp.SessionKey);
								nextActivity.PutExtra("GroupID", loginResp.GroupID);
								nextActivity.PutExtra("UserID", loginResp.UserID);
								StartActivity(nextActivity);
								Finish();
							}
						} else {
							var myHandler = new Handler();
							myHandler.Post(() => {
								Android.Widget.Toast.MakeText(this, "Login Failed. Please Try Again.", Android.Widget.ToastLength.Long).Show();
							});
						}
					} catch (Exception e) {
						var myHandler = new Handler();
						myHandler.Post(() => {
							Android.Widget.Toast.MakeText(this, e.Message, Android.Widget.ToastLength.Long).Show();
						});
						System.Diagnostics.Debug.Write(e.Message);
					}
				}
			};

		}

		public override void OnBackPressed() {
			var builder = new Android.App.AlertDialog.Builder(this);
			builder.SetTitle ("Exit.");
			builder.SetIcon (Android.Resource.Drawable.IcDialogAlert);
			builder.SetMessage("Exit App?");
			builder.SetPositiveButton("OK", (s, e) => { base.OnStop; });
			builder.SetNegativeButton("Cancel", (s, e) => { });
			builder.Create().Show();
		}
	}
}


