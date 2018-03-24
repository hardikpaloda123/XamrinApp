using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestApp
{
    #region Classes

    public class ProfileInfo
    {
        public string ProfileId;

        public string ImageId;

        public Name Name { get; set; }

        public Location Location { get; set; }
    }

    public class Name
    {
        public string First { get; set; }

        public string Last { get; set; }

        public string Middle { get; set; }
    }

    public class Location
    {
        public string Location_Name { get; set; }

        public LatLng Coords { get; set; }
    }

    public class LatLng
    {
        public string Longitude { get; set; }

        public string Lattitude { get; set; }
    }

    public class DataFormat
    {
        public ProfileInfo ProfileInfo { get; set; }

        public List<ProfileInfo> Followers { get; set; }
    }

    #endregion Classes

    public class Models : ObservableObject
    {
        private string jsonObject;

        public string JsonObject
        {
            get { return jsonObject; }

            set { SetProperty(ref jsonObject, value); }
        }

        public DataFormat Data = new DataFormat
        {
            ProfileInfo = new ProfileInfo
            {
                Name = new Name(),
                Location = new Location
                {
                    Coords = new LatLng()
                }
            },
            Followers = new List<TestApp.ProfileInfo>()
        };

        #region Constructor

        public Models()
        {
            var data = "profile|73241234|<Niharika><><Khan>|<Mumbai><<72.872075><19.075606>>|73241234" +
                       "jpg**followers|54543343|<Amitabh><><>|<Dehradun><<><>>|54543343.jpg@@|22112211 |<Piyush><><>||";

            var length = data.Length;

            //Split the query string in profile info and followers
            int StartOfFollower = data.IndexOf("followers");

            //Trim the profile info for profile object properties
            var personInfo = data.Substring(1, StartOfFollower - 1);

            //Trim the followers info
            var follwersStr = data.Substring(StartOfFollower, length - StartOfFollower - 1);

            //Get the profileInfo properties in string properties seperates by !
            var str = MapNameLocatoin(personInfo).Split('!');

            //Assign the String properties in ProfileInfo Object
            Data.ProfileInfo = MapStringToObject(str);

            //Split the followers list
            var followersObjs = follwersStr.Split(new[] { "jpg@@" }, StringSplitOptions.None);

            //Run the loop to convert each list info in object and add object to followers list
            for (int i = 0; i < followersObjs.Length; i++)
            {
                //Get the string seperated by !
                var propArray = MapNameLocatoin(followersObjs[i]).Split('!');

                //Create a object by assignig info to properties in string
                ProfileInfo Data1 = MapStringToObject(propArray);

                //Add object to List.
                Data.Followers.Add(Data1);
            }

            //Used Newtonsoft Nuget Package to get the json format of given object
            JsonObject = JsonConvert.SerializeObject(Data);

            //Can Also See On Logs.
            Debug.WriteLine(JsonObject + "RequiredData");
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Used to assign the properties in string to object
        /// </summary>
        /// <param name="propArray"></param>
        /// <returns></returns>
        private static ProfileInfo MapStringToObject(string[] propArray)
        {
            var Data1 = new ProfileInfo
            {
                Name = new Name(),
                Location = new Location
                {
                    Coords = new LatLng()
                }
            };

            Data1.ProfileId = propArray[0];

            //If all the array values (1,2,3) are null or Empty means Name object is null.
            if (String.IsNullOrEmpty(propArray[1]) && String.IsNullOrEmpty(propArray[3]) && String.IsNullOrEmpty(propArray[2]))
            {
                Data1.Name = null;
            }
            else
            {
                Data1.Name.First = propArray[1];

                Data1.Name.Middle = propArray[2];

                Data1.Name.Last = propArray[3];
            }

            //If all the array values (4,5,6) are null or Empty means LOcation object is null.

            if (String.IsNullOrEmpty(propArray[4]) && String.IsNullOrEmpty(propArray[5]) && String.IsNullOrEmpty(propArray[6]))
            {
                Data1.Location = null;
            }
            else
            {
                Data1.Location.Location_Name = propArray[4];

                Data1.Location.Coords.Lattitude = propArray[5];

                Data1.Location.Coords.Longitude = propArray[6];
            }

            Data1.ImageId = propArray[7];

            ///   Return Object  with assigned properties.

            return Data1;
        }

        private string MapNameLocatoin(string nameLocation)
        {
            var arrayNameLocation = nameLocation.Split('|');

            var profileId = arrayNameLocation[1];

            var Namearray = arrayNameLocation[2];

            //If name array lenght is less than 0 means Name object is null than pass all properties as empty and assign
            //Object as null(Refer line 125) else call MapName
            var strname = Namearray.Length <= 0 ? "" + "!" + "" + "!" : MapName(Namearray);

            var locationInfo = arrayNameLocation[3];

            //If locationInfo array lenght is less than 0 means Location object is null than pass all properties as empty and assign
            //Object as null(Refer line 140) else call arrayNameLocation
            var strlocations = locationInfo.Length <= 0 ? "" + "!" + "" + "!" : MapLocations(locationInfo);

            var imageId = arrayNameLocation.Length < 5 ? "" : arrayNameLocation[4];

            //Pass all information in string seperated by !.
            var x = profileId + "!" + strname + "!" + strlocations + "!" + imageId;

            ///   return  LOcation object properties in string seperated by '!'

            return x;
        }

        /// <summary>
        ///  used to give the location details in strig seperated by !
        ///
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        private string MapLocations(string locationInfo)
        {
            var locationInfoArray = locationInfo.Split(new[] { "><" }, StringSplitOptions.None);

            //Handle the scenarios when name is absent
            var loc_Name = locationInfoArray[0].Length <= 1 ? "" : locationInfoArray[0].Substring(1, locationInfoArray[0].Length - 1);

            //Handle the scenarios when lattitude is absent
            var cord_lat = locationInfoArray[1].Length <= 1 ? "" : locationInfoArray[1].Substring(1, locationInfoArray[1].Length - 1);

            //Handle the scenarios when longitude is absent
            var cord_Long = locationInfoArray[2].Length <= 2 ? "" : locationInfoArray[2].Substring(0, locationInfoArray[2].Length - 3);

            return loc_Name + "!" + cord_lat + "!" + cord_Long;
        }

        /// <summary>
        /// Used to give the Name details in strig seperated by !
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string MapName(string item)
        {
            var len = item.Length;

            var str = item.Split(new[] { "><" }, StringSplitOptions.None);

            //Handle the scenarios when first name is absent
            var fname = str[0].Length < 1 ? "" : str[0].Substring(1, str[0].Length - 1);

            var mname = str[1];

            //Handle the scenarios when last name is absent
            var lname = str[2].Length <= 1 ? "" : str[2].Substring(0, str[2].Length - 1);

            ///   return  Name object properties in string seperated by '!'
            return fname + "!" + mname + "!" + lname;
        }

        #endregion Methods
    }
}