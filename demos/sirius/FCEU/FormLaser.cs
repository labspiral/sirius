﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SpiralLab.Sirius.FCEU
{
    public partial class FormLaser : Form
    {
        public SiriusEditorForm SiriusEditor
        {
            get
            {
                return this.siriusEditorForm1;
            }
        }
        public FormLaser()
        {
            InitializeComponent();

        }


    }
}