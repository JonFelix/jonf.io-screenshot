﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jonf.io_screenshot
{
    [Flags]
    public enum ModifierKeys
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8,
        Noone = 0

    }

    public struct ImageItem
    {
        public Bitmap Image;
        public DateTime Time;
    }


    public struct ScreenSettings
    {
        public string Name;
        public bool SaveFile;
        public PictureBox Image;
        public List<ImageItem> History;
    }

    public partial class Form1 : Form
    {                             
        Screenshot _Screenshot;
        KeyboardHook _KeyboardHook;
        ScreenSettings[] _ScreenSettings;
        string _SaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);


        public Form1()
        {
            InitializeComponent();
            _Screenshot = new Screenshot(Screen.AllScreens);
            _KeyboardHook = new KeyboardHook();
            _ScreenSettings = new ScreenSettings[Screen.AllScreens.Length];

            _KeyboardHook.KeyPressed +=
            new EventHandler<KeyPressedEventArgs>(HookKeyPressed);
            // register the control + alt + F12 combination as hot key.
            _KeyboardHook.RegisterHotKey(jonf.io_screenshot.ModifierKeys.Noone, Keys.PrintScreen);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Screen[] _Screens = Screen.AllScreens;                        
            for(int i = 0; i < _Screens.Length; i++)
            {
                _ScreenSettings[i].Name = _Screens[i].DeviceName;
                _ScreenSettings[i].SaveFile = true;
                _ScreenSettings[i].History = new List<ImageItem>();
                TabPage tab = new TabPage(_Screens[i].DeviceName);
                _ScreenSettings[i].Image = new PictureBox();
                _ScreenSettings[i].Image.Dock = DockStyle.Fill;
                _ScreenSettings[i].Image.SizeMode = PictureBoxSizeMode.StretchImage;
                tab.Controls.Add(_ScreenSettings[i].Image);
                tabControl1.TabPages.Add(tab);
                ToolStripMenuItem menuItem = new ToolStripMenuItem(_Screens[i].DeviceName);
                menuItem.Checked = true;
                menuItem.CheckOnClick = true;
                menuItem.Click += (sender1, eventArgs) =>
                {
                    for(int z = 0; z < _ScreenSettings.Length; z++)
                    {
                        if(_ScreenSettings[z].Name == ((ToolStripMenuItem)sender1).Text)
                        {
                            _ScreenSettings[z].SaveFile = ((ToolStripMenuItem)sender1).CheckState == CheckState.Checked;
                        }
                    }                  
                };
                MenuSave.DropDownItems.Add(menuItem);
              
            }

        }

        private void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            
        }

        void HookKeyPressed(object sender, KeyPressedEventArgs e)
        {
            for(int i = 0; i < Screen.AllScreens.Length; i++)
            {
                _ScreenSettings[i].Image.Image = _Screenshot.TakeScreenhot(i);
                ImageItem historyItem = new ImageItem();
                historyItem.Image = (Bitmap)_ScreenSettings[i].Image.Image;
                historyItem.Time = DateTime.Now;
                _ScreenSettings[i].History.Add(historyItem);
            }
            ListViewItem lviItem = new ListViewItem();
            lviItem.Tag = _ScreenSettings[0].History.Count - 1;
            lviItem.Text = DateTime.Now.ToShortTimeString();
            listView1.Items.Add(lviItem);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HookKeyPressed(sender, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < _ScreenSettings.Length; i++)
            {
                if(_ScreenSettings[i].SaveFile)
                {   
                    _Screenshot.SaveScreenshot(_SaveLocation + @"\" + @DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace("PM", "").Replace("AM", "").Replace(" ", "")+"_"+i.ToString(), (Bitmap)_ScreenSettings[i].Image.Image);
                }
            }
        } 

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {   
                for(int i = 0; i < _ScreenSettings.Length; i++)
                {
                    _ScreenSettings[i].Image.Image = _ScreenSettings[i].History[(int)listView1.SelectedItems[0].Tag].Image;
                }
            }
        }
    }
}
