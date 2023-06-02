using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace LCR.Models
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int _Games { get; set; } // # of games selected for simulation
        private int _Players { get; set; } // # of players selected for simulation
        private string _Status { get; set; } // only there to indicate what the system is currently doing
        private int _Selection { get; set; } // the selected index of the pre-set options on the combo box

        private List<string> _CaptionList { get; set; } = new List<string>();

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

        public List <string> CaptionList
        {
            get { return _CaptionList; }
            set { _CaptionList = value; OnPropertyChanged("CaptionList"); }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

        public int Games
        {
            get { return _Games; }
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }

        // ********************************************************************

        public void GetParameters(string Str)
        {   // this function parses the selected string in the combo box into _Players and _Games         
            int End1, End2, i, Start1, Start2;
            string SubStr1, SubStr2;

            Start1 = 0;
            for (i = 0; i < Str.Length; i++)
            {
                if (Str.Substring(i, 1) == "=")
                {
                    Start1 = i + 2; // points to first digot of Players value in string
                    break;
                }
            }

            End1 = Start1 + 1;
            for (i=Start1; i<Str.Length; i++)
            {
                if (Str.Substring(i, 1) == ",")
                {
                    End1 = i;
                    break;
                }
            }

            SubStr1 = Str.Substring(Start1, End1 - Start1);

            Start2 = 0;
            for (i = End1; i < Str.Length; i++)
            {
                if (Str.Substring(i, 1) == "=")
                {
                    Start2 = i + 2; // points to first digot of Games value in string
                    break;
                }
            }
            End2 = Str.Length;
            SubStr2 = Str.Substring(Start2, End2 - Start2);

            _Players = Convert.ToInt32(SubStr1);
            _Games = Convert.ToInt32(SubStr2);
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

        public int Players
        {
            get { return _Players; }
            set
            {
                _Players = value;
                OnPropertyChanged("Players");
            }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

        public int Selection
        {            
            get { return _Selection; }
            set
            {
                _Selection = value;
                OnPropertyChanged("Selection");
                GetParameters(_CaptionList[_Selection]); // This function parses the caption string into _Players and _Games
                OnPropertyChanged("Players");
                OnPropertyChanged("Games");
            }
        }

        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public MainWindowModel(string Caption)
        {
            _Status = Caption;
            _Players = 3; // initial option
            _Games = 100; // initial option

            // This block adds the pre-set options to the combo box
            _CaptionList.Add("Players = 3, Games = 100"); 
            _CaptionList.Add("Players = 4, Games = 100");
            _CaptionList.Add("Players = 5, Games = 100");
            _CaptionList.Add("Players = 5, Games = 1000");
            _CaptionList.Add("Players = 5, Games = 10000");
            _CaptionList.Add("Players = 5, Games = 100000");
            _CaptionList.Add("Players = 6, Games = 100");
            _CaptionList.Add("Players = 7, Games = 100");

            _Selection = 0; // making the default selection the first one
        }

        // ********************************************************************

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}