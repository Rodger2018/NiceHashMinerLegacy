﻿using NiceHashMiner.Interfaces;
using NiceHashMiner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NiceHashMiner
{
    public partial class Form_Loading : Form, IMessageNotifier, IMinerUpdateIndicator
    {
        public interface IAfterInitializationCaller {
            void AfterLoadComplete();
        }

        private int LoadCounter = 0;
        // TODO find out what are the 13 loading steps, think if this should really be hardcoded
        private int TotalLoadSteps = 12;
        private readonly IAfterInitializationCaller AfterInitCaller;

        public Form_Loading(IAfterInitializationCaller initCaller, string loadFormTitle, string startInfoMsg, int totalLoadSteps)
        {
            InitializeComponent();

            label_LoadingText.Text = loadFormTitle;
            label_LoadingText.Location = new Point((this.Size.Width - label_LoadingText.Size.Width) / 2, label_LoadingText.Location.Y);

            // TODO assert that this is never null
            AfterInitCaller = initCaller;

            TotalLoadSteps = totalLoadSteps;
            this.progressBar1.Maximum = TotalLoadSteps;
            this.progressBar1.Value = 0;

            SetInfoMsg(startInfoMsg);
        }

        // download miners
        public Form_Loading() {
            InitializeComponent();
            label_LoadingText.Location = new Point((this.Size.Width - label_LoadingText.Size.Width) / 2, label_LoadingText.Location.Y);
            _startMinersInitLogic = true;
        }

        public void IncreaseLoadCounterAndMessage(string infoMsg) {
            SetInfoMsg(infoMsg);
            IncreaseLoadCounter();
        }

        // TODO maybe remove this method
        public void SetProgressMaxValue(int maxValue) {
            this.progressBar1.Maximum = maxValue;
        }
        public void SetInfoMsg(string infoMsg) {
            this.LoadText.Text = infoMsg;
        }

        public void IncreaseLoadCounter() {
            LoadCounter++;
            this.progressBar1.Value = LoadCounter;
            this.Update();
            if (LoadCounter >= TotalLoadSteps) {
                AfterInitCaller.AfterLoadComplete();
                this.Close();
                this.Dispose();
            }
        }

        public void FinishLoad() {
            while (LoadCounter < TotalLoadSteps) {
                IncreaseLoadCounter();
            }
        }

        public void SetValueAndMsg(int setValue, string infoMsg) {
            SetInfoMsg(infoMsg);
            progressBar1.Value = setValue;
            this.Update();
            if (progressBar1.Value >= progressBar1.Maximum) {
                this.Close();
                this.Dispose();
            }
        }

        #region IMessageNotifier
        public void SetMessage(string infoMsg) {
            SetInfoMsg(infoMsg);
        }

        public void SetMessageAndIncrementStep(string infoMsg) {
            IncreaseLoadCounterAndMessage(infoMsg);
        }
        #endregion //IMessageNotifier

        #region IMinerUpdateIndicator
        public void SetMaxProgressValue(int max) {
            this.Invoke((MethodInvoker)delegate {
                this.progressBar1.Maximum = max;
                this.progressBar1.Value = 0;
            });
        }

        public void SetProgressValueAndMsg(int value, string msg) {
            if (value <= this.progressBar1.Maximum) {
                this.Invoke((MethodInvoker)delegate {
                    this.progressBar1.Value = value;
                    this.LoadText.Text = msg;
                    this.progressBar1.Invalidate();
                    this.LoadText.Invalidate();
                });
            }
        }

        public void SetTitle(string title) {
            this.Invoke((MethodInvoker)delegate {
                label_LoadingText.Text = title;
            });
        }

        public void FinishMsg(bool success) {
            this.Invoke((MethodInvoker)delegate {
                //if (success) {
                //    label_LoadingText.Text = "Init Finished!";
                //} else {
                //    label_LoadingText.Text = "Init Failed!";
                //}
                System.Threading.Thread.Sleep(1000);
                Close();
            });
        }

        #endregion IMinerUpdateIndicator

        bool _startMinersInitLogic = false;
        private void Form_Loading_Shown(object sender, EventArgs e) {
            if (_startMinersInitLogic) {
                MinersDownloadManager.Instance.Start(this);
            }
        }
    }
}
