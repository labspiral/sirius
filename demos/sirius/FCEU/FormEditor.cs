﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Numerics;

namespace SpiralLab.Sirius.FCEU
{
    public partial class FormEditor : Form
    {
        public SiriusEditorForm SiriusEditor
        {
            get
            {
                return this.siriusEditorForm1;
            }
        }
        public FormEditor()
        {
            InitializeComponent();
            this.SiriusEditor.OnCorrection2D += SiriusEditor_OnCorrection2D;
        }
      
        private void SiriusEditor_OnCorrection2D(object sender, EventArgs e)
        {
            //example codes
            var rtc = this.SiriusEditor.Rtc;

            int rows = 5;
            int cols = 5;

            float interval = 10.0f;
            var correction2D = new RtcCorrection2D(rtc.KFactor, rows, cols, interval, rtc.CorrectionFiles[0], string.Empty);
            float left = -interval * (float)(int)(cols / 2);
            float top = interval * (float)(int)(rows / 2);
            var rand = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    correction2D.AddRelative(row, col,
                        new Vector2(
                            left + col * interval,
                            top - row * interval),
                        new Vector2(
                            rand.Next(20) / 1000.0f - 0.01f,
                            rand.Next(20) / 1000.0f - 0.01f)
                        );
                }
            }
            var form2D = new Correction2DForm(correction2D);
            form2D.OnApply += Form2D_OnApply;
            form2D.ShowDialog(this);
        }

        private void Form2D_OnApply(object sender, EventArgs e)
        {
            var form = sender as Correction2DForm;
            string ct5FileName = form.RtcCorrection.TargetCorrectionFile;
            if (!File.Exists(ct5FileName))
                return;
            var mb = new MessageBoxYesNo();
            if (DialogResult.Yes != mb.ShowDialog("Warning !", "Do you really want to apply new correction file ?"))
                return;
            var rtc = this.SiriusEditor.Rtc;
            bool success = true;
            success = rtc.CtlLoadCorrectionFile(CorrectionTableIndex.Table1, ct5FileName);
            success = rtc.CtlSelectCorrection(CorrectionTableIndex.Table1);
            if (success)
            {
                //update ini file
                var iniFileName = FormMain.ConfigFileName;
                NativeMethods.WriteIni<string>(iniFileName, $"RTC", "CORRECTION", Path.GetFileName(ct5FileName));
                var mb2 = new MessageBoxOk();
                mb2.ShowDialog("Correction", $"Correction file has changed to {iniFileName}");
            }
        }

    }
}