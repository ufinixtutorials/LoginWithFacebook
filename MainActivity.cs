using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Facebook.Login.Widget;
using System.Collections.Generic;
using Xamarin.Facebook;
using Java.Lang;
using Firebase.Auth;
using Firebase;
using Android.Content;
using Xamarin.Facebook.Login;
using Android.Gms.Tasks;

namespace LoginWithFacebook
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IFacebookCallback, IOnSuccessListener, IOnFailureListener
    {
        TextView usernameText, emailText, photoText;
        LoginButton facebookLoginButton;
        ICallbackManager callBackManager;

        FirebaseAuth firebaseAuth;
        private bool usingFirebase;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            usernameText = (TextView)FindViewById(Resource.Id.usernameText);
            emailText = (TextView)FindViewById(Resource.Id.emailText);
            photoText = (TextView)FindViewById(Resource.Id.photoText);


            facebookLoginButton = (LoginButton)FindViewById(Resource.Id.loginButton);
            facebookLoginButton.SetReadPermissions(new List<string> { "public_profile", "email" });

            callBackManager = CallbackManagerFactory.Create();
            facebookLoginButton.RegisterCallback(callBackManager, this);
            firebaseAuth = GetFirebaseAuth();
        }

        FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);
            FirebaseAuth mauth;

            if(app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("loginwithfacebook-765f0")
                    .SetApplicationId("loginwithfacebook-765f0")
                    .SetApiKey("AIzaSyAo5NEeyalKyGp6JXH4EFYZzeyXIFPGE40")
                    .SetDatabaseUrl("https://loginwithfacebook-765f0.firebaseio.com")
                    .SetStorageBucket("loginwithfacebook-765f0.appspot.com")
                    .Build();

                app = FirebaseApp.InitializeApp(this, options);
                mauth = FirebaseAuth.Instance;
            }
            else
            {
                mauth = FirebaseAuth.Instance;
            }

            return mauth;
            
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        public void OnCancel()
        {
        }

        public void OnError(FacebookException error)
        {
        }

        public void OnSuccess(Object result)
        {
            if (!usingFirebase)
            {
                usingFirebase = true;
                LoginResult loginResult = result as LoginResult;
                var credentials = FacebookAuthProvider.GetCredential(loginResult.AccessToken.Token);
                firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
            }
            else
            {
                Toast.MakeText(this, "Login successful", ToastLength.Short).Show();
                usingFirebase = false;

                emailText.Text = "Email: " + firebaseAuth.CurrentUser.Email;
                photoText.Text = "PhotoURL: " + firebaseAuth.CurrentUser.PhotoUrl.Path;
                usernameText.Text ="Display Name: " + firebaseAuth.CurrentUser.DisplayName;
            }
          
        }

        public void OnFailure(Exception e)
        {

        }
    }
}