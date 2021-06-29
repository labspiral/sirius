using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiralLab.Sirius.FCEU
{
    
    public class LaserService  
        : SpiralLab.IServiceLaser
    {
        public string Name { get; set; }
        public object Tag { get; set; }
        public int RecipeNo { get; internal set; }
        public bool IsVisionConnected
        {
            get
            {
                return seq.VisionComm.IsConnected;
            }
        }

        public int FieldCorrectionRows { get; set; }
        public int FieldCorrectionCols { get; set; }
        public float FieldCorrectionInterval { get; set; }

        LaserSequence seq;

        public LaserService(LaserSequence seq)
        {
            this.Name = "FCEU Laser Service";
            this.seq = seq;
            this.RecipeClear();
        }

        public void RecipeClear()
        {
            RecipeNo = -1;
            //var doc = new Sirius.DocumentDefault();
            var doc = new DocumentFCEU();

            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                seq.Viewer.Document = doc;
                seq.Editor.Document = doc;

                //기본 펜 생성
                var pen = new PenDefault();
                doc.Action.ActEntityAdd(pen);

            }));
        }
        public bool RecipeChange(int no)
        {
            if (seq.IsBusy)
            {
                seq.Error(ErrEnum.Busy);
                Logger.Log(Logger.Type.Error, $"try to change recipe buy busy !");
                return false;
            }
            this.RecipeClear();
            bool success = true;
            string recipeFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recipes", $"{no}", "laser.sirius");
            if (!File.Exists(recipeFileName))
            {
                seq.Error(ErrEnum.Recipe);
                return false;
            }
            string iniFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recipes", "recipe.ini");
            string recipeName = NativeMethods.ReadIni<string>(iniFileName, $"{no}", "NAME");
            var form = new ProgressForm()
            {
                Message = $"Loading Recipe : [{no}] {recipeName}" + Environment.NewLine,
                Percentage = 0,
            };
            Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
            {
                form.Show(seq.Editor);
            }));
            var doc = DocumentSerializer.OpenSirius(recipeFileName);
            if (null == doc)
            {
                seq.Error(ErrEnum.Recipe);
                Logger.Log(Logger.Type.Warn, $"fail to change recipe to [RecipeNo]: {recipeName}");
                Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                {
                    form.Close();
                }));
                return false;
            }

            var markerArg = new MarkerArgDefault();
            markerArg.Document = doc;
            markerArg.Rtc = seq.Rtc;
            markerArg.Laser = seq.Laser;
            markerArg.ScannerRotateAngle = NativeMethods.ReadIni<float>(FormMain.ConfigFileName, $"RTC", "ROTATE");
            Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
            {
                form.Percentage = 50;
            }));
            success &= seq.Marker.Ready(markerArg);
            if (success)
            {
                Program.MainForm.Invoke(new MethodInvoker(delegate ()
                {
                    Application.DoEvents();
                    seq.Editor.Document = doc;
                    seq.Editor.FileName = recipeFileName;
                    Application.DoEvents();
                    //Viewer.Document = doc; 자동 !
                    Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        form.Percentage = 100;
                    }));
                }));
                RecipeNo = no;
                seq.Warn(WarnEnum.RecipeChanged);
                Logger.Log(Logger.Type.Warn, $"recipe has changed to [{no}]: {recipeName}");
            }
            else
            {
                seq.Error(ErrEnum.Recipe);
                Logger.Log(Logger.Type.Warn, $"fail to change recipe to [{no}]: {recipeName}");
            }
            Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
            {
                form.Close();
            }));
            return success;
        }

        public bool ReadScannerFieldCorrection()
        {
            if (seq.isFieldCorrecting)
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"another field correction form is activating");
                return false;
            }
            //비전에서 기록한 보정 측정 정보
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup", "scannerfieldcorrection.txt");
            if (false == File.Exists(fileName))
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"try to open field correction file but failed : {fileName}");
                return false;
            }
            // data
            int rows = 9;
            int cols = 9;
            float interval = 10;

            var correction2D = new RtcCorrection2D(seq.Rtc.KFactor, rows, cols, interval, seq.Rtc.CorrectionFiles[0], string.Empty);
            var form2D = new Correction2DForm(correction2D);
            form2D.OnApply += Form2D_OnApply;
            form2D.OnClose += Form2D_OnClose;
            seq.isFieldCorrecting = true; //ready off
            seq.Warn(WarnEnum.ScannerFieldCorrectioning);
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form2D.Show(seq.Editor);
            }));
            return true;
        }
        private void Form2D_OnApply(object sender, EventArgs e)
        {
            var form = sender as Correction2DForm;
            string ct5FileName = form.RtcCorrection.TargetCorrectionFile;
            if (!File.Exists(ct5FileName))
            {
                Logger.Log(Logger.Type.Error, $"target correction file is not exist : {ct5FileName}");
                return;
            }
            var mb = new MessageBoxYesNo();
            if (DialogResult.Yes != mb.ShowDialog("Warning !", $"Do you really want to apply new correction file {ct5FileName} ?"))
                return;
            var rtc = seq.Rtc;
            bool success = true;
            success &= rtc.CtlLoadCorrectionFile(CorrectionTableIndex.Table1, ct5FileName);
            success &= rtc.CtlSelectCorrection(CorrectionTableIndex.Table1);
            if (success)
            {
                //update ini file
                var iniFileName = FormMain.ConfigFileName;
                NativeMethods.WriteIni<string>(iniFileName, $"RTC", "CORRECTION", Path.GetFileName(ct5FileName));
                seq.Warn(WarnEnum.ScannerFieldCorrectionChanged);
                Logger.Log(Logger.Type.Warn, $"Correction file has changed to {ct5FileName}");
                var mb2 = new MessageBoxOk();
                mb2.ShowDialog("Correction", $"Correction file has changed to {ct5FileName}");
            }
        }
        private void Form2D_OnClose(object sender, EventArgs e)
        {
            seq.isFieldCorrecting = false;
            seq.Warn(WarnEnum.ScannerFieldCorrectioning, true);
        }

        /// <summary>
        /// 화면 편집기 업데이트
        /// </summary>
        /// <param name="index">1(오른쪽), 2(왼쪽)</param>
        /// <param name="group">그룹 객체들</param>
        /// <returns></returns>
        public bool PrepareDefectInEditor(int index, Group group)
        {
            // 오토 화면 
            //if (this.formMain.FormCurrent != this.formMain.FormAuto)
            //    return false;

            var doc = seq.Editor.Document; //에디터의 doc 를 대상으로
            var name = $"Defect{index}";
            var layer = doc.Layers.NameOf(name);
            if (null == layer || 0 == layer.Count || !(layer[0] is IPen))
            {
                seq.Error(ErrEnum.NoDefectLayer);
                Logger.Log(Logger.Type.Error, $"target layer or pen entity is not exist : {name}");
                return false;
            }

            Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
            {
                var form = new ProgressForm()
                {
                    Message = $"Creating Defect Data ..." + Environment.NewLine,
                    Percentage = 0,
                };
                form.Show(seq.Editor);
                var deleteEntities = new List<IEntity>(layer.Count);
                foreach (var entity in layer)
                {
                    if (!(entity is IPen))
                        deleteEntities.Add(entity);
                }
                form.Percentage = 10;
                Application.DoEvents();
                doc.Action.ActEntityDelete(deleteEntities);
                Application.DoEvents();
                doc.Action.ActEntityAdd(group, layer);
                Application.DoEvents();
                form.Message += "Adding To Document ..." + Environment.NewLine;
                doc.Action.SelectedEntity = null;
                form.Percentage = 50;
                Application.DoEvents();
                doc.Action.UndoRedoClear(); //100 undo 개수 제한이 있지만 메모리 소비가 클수있음 ? save memory
                form.Message += $"Regening ..." + Environment.NewLine;
                Application.DoEvents();
                Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                {
                    seq.Editor.View.Render();
                }));
                form.Percentage = 100;
                for (int i=0; i< 100; i++)
                    Application.DoEvents();
                form.Close();
            }));
            return true;
        }
        public bool PrepareDefectInMarker(int index, Group group)
        {
            // 오토 화면 
            //if (this.formMain.FormCurrent != this.formMain.FormAuto)
            //    return false;
            if (seq.IsBusy)
            {
                seq.Error(ErrEnum.Busy);
                Logger.Log(Logger.Type.Error, $"trying to change defect info into marker but busy");
                return false;
            }

            //마커 doc 의 동기화는? 수정할 수 있는 타이밍이 존재?
            var doc = seq.Marker.Document; //복제된 doc 를 대상으로
            var name = $"Defect{index}";
            var layer = doc.Layers.NameOf(name);
            if (null == layer || 0 == layer.Count || !(layer[0] is IPen))
            {
                seq.Error(ErrEnum.NoDefectLayer);
                Logger.Log(Logger.Type.Error, $"target layer or pen entity is not exist : {name}");
                return false;
            }
            //Program.MainForm.Invoke(new MethodInvoker(delegate ()
            //{
                // 첫번째 객체를 제외하고 모두 삭제
                var deleteEntities = new List<IEntity>(layer.Count);
                foreach (var entity in layer)
                {
                    if (!(entity is IPen))
                        deleteEntities.Add(entity);
                }
                doc.Action.ActEntityDelete(deleteEntities);
                doc.Action.ActEntityAdd(group, layer);
                doc.Action.UndoRedoClear(); //100 undo 개수 제한이 있지만 메모리 소비가 클수있음 ? save memory
            //}));
            return true;
        }
        public bool ReadDefectFromFile(string fileName, out Group group)
        {
            ProgressForm form = new ProgressForm();
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form.Message = $"Reading From Defect File: {fileName}";
                form.Percentage = 0;
                form.Show(seq.Editor);
            }));

            bool success = true;
            group = new Group();
            group.Name = $"Defects";
            group.IsEnableFastRendering = true;
            group.IsHitTest = false; //선택 않되도록
            //if (!File.Exists(fileName))
            //{
            //      form.Close();
            //    seq.Error(ErrEnum.VisionDataOpen);
            //    Logger.Log(Logger.Type.Error, $"fail to open vision defect file : {fileName}");
            //    return false;
            //}
            int counts = 0;
            seq.Warn(WarnEnum.VisionDataOpen);

            try
            {
                //using (var stream = new StreamReader(fileName))
                //{
                //    while (!stream.EndOfStream)
                //    {
                //        var line = stream.ReadLine();
                //        if (string.IsNullOrEmpty(line))
                //            continue;

                //if (line.ElementAt(0) == ';')
                //    continue;
                //string[] tokens = line.Split(',');
                //float x = float.Parse(tokens[0]);
                //float y = float.Parse(tokens[1]);
                //float angle = float.Parse(tokens[2]);

                //var poly = new LwPolyline();
                //poly.Add(new LwPolyLineVertex(55.3005f, 125.1903f, 0));
                //poly.Add(new LwPolyLineVertex(80.5351f, 161.2085f, 0));
                //poly.Add(new LwPolyLineVertex(129.8027f, 148.6021f, -1.3108f));
                //poly.Add(new LwPolyLineVertex(107.5722f, 109.5824f, 0.8155f));
                //poly.Add(new LwPolyLineVertex(77.5310f, 89.7724f, 0));
                //poly.IsClosed = true;
                //poly.IsHatchable = true;
                //poly.HatchAngle = 90;
                //poly.HatchInterval = 0.2f;
                //poly.HatchExclude = 0.1f;
                //poly.Regen();
                //Program.MainForm.Invoke(new MethodInvoker(delegate ()
                //{
                //    editor.Document.Action.ActEntityAdd(poly);
                //    editor.View.Render();

                //}));
                //counts++;
                //}
                //}

                
                var doc = seq.Editor.Document; //에디터의 doc 를 대상
                var docFceu = doc as DocumentFCEU;//해치 정보 조회하기 위해 접근

                int max = 1000;
                Random rand = new Random();
                for (int i = 0; i < max; i++)
                {
                    var poly = new LwPolyline();
                    poly.Add(new LwPolyLineVertex(5.300f, 10.190f, 0));
                    poly.Add(new LwPolyLineVertex(6.535f ,10.208f , 0));
                    poly.Add(new LwPolyLineVertex(6.572f, 10.582f, -0.815f));
                    poly.Add(new LwPolyLineVertex(5.531f, 10.772f * 0));
                    poly.IsClosed = true;
                    if (null != docFceu)
                    {
                        poly.IsHatchable = docFceu.IsHatchable;
                        poly.HatchAngle = docFceu.HatchAngle;
                        poly.HatchInterval = docFceu.HatchInterval;
                        poly.HatchExclude = docFceu.HatchExclude;
                    }
                    poly.Regen();
                    poly.Transit(new System.Numerics.Vector2( -300* (float)rand.NextDouble()+150, -80 * (float)rand.NextDouble()+ 35));
                    group.Add(poly);

                    Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        form.Percentage = i / max * 100;
                    }));
                }

                Logger.Log(Logger.Type.Info, $"success to open defect file : {counts} counts at {fileName}");
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Type.Error, $"fail to open defect file : {fileName} : {ex.Message}");
                success &= false;
            }
            finally
            {
                if (success)
                {
                    Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        form.Message = $"Regening ... ";
                    }));
                    group.Regen();
                    //x 정렬 오름차순
                    group.Sort(delegate (IEntity e1, IEntity e2)
                    {
                        return e1.BoundRect.Center.X.CompareTo(e2.BoundRect.Center.X);
                    });
                    seq.Warn(WarnEnum.VisionDataOpen, true);
                }
                Program.MainForm.BeginInvoke(new MethodInvoker(delegate ()
                {
                    form.Close();
                }));
            }
            return success;
        }

    }
}