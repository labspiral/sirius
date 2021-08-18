using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpiralLab.Sirius.FCEU
{
    /// <summary>
    /// 레이저 서비스 객체
    /// </summary>
    public class LaserService
        : SpiralLab.IServiceLaser
    {
        public string Name { get; set; }
        public object Tag { get; set; }
        public int RecipeNo { get; internal set; }
        public string RecipeName { get; internal set; }
        public bool IsVisionConnected
        {
            get
            {
                return seq.VisionComm.IsConnected;
            }
        }
        public int FieldCorrectionRows { get; set; }
        public int FieldCorrectionCols { get; set; }
        public float FieldCorrectionRowInterval { get; set; }
        public float FieldCorrectionColInterval { get; set; }
        //좌 -> 우
        public float ZCorrectionDx { get; set; }
        public float ZCorrectionDy { get; set; }
        public float ZCorrectionCount { get; set; }
        public int ZCorrectionIndex { get; set; }

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
            RecipeName = string.Empty;
            var doc = new Sirius.DocumentDefault();

            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                seq.Editor.Document = doc;
                //seq.Viewer.Document = doc;

                //사용자 정의 펜 생성
                //var pen = new FCEUPen();
                //seq.Editor.OnDocumentPenNew += SiriusEditorForm1_OnDocumentPenNew;
                //기본 펜 생성 
                var pen = new PenDefault();
                doc.Action.ActEntityAdd(pen);
            }));

        }
        //private void SiriusEditorForm1_OnDocumentPenNew(object sender)
        //{
        //    // 사용자 정의 펜 엔티티를 생성
        //    var pen = new FCEUPen();
        //    seq.Editor.OnPenNew(pen);
        //}
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
                Logger.Log(Logger.Type.Error, $"fail to change recipe to [{no}]: {recipeFileName}");
                seq.Error(ErrEnum.RecipeChange);
                return false;
            }
            string iniFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recipes", "recipe.ini");
            string recipeName = NativeMethods.ReadIni<string>(iniFileName, $"{no}", "NAME");
            var form = new ProgressForm()
            {
                Message = $"Loading Recipe : [{no}] {recipeName}" + Environment.NewLine,
                Percentage = 0,
            };
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form.Show(seq.Editor);
            }));
            var doc = DocumentSerializer.OpenSirius(recipeFileName);
            if (null == doc)
            {
                seq.Error(ErrEnum.RecipeChange);
                Logger.Log(Logger.Type.Error, $"fail to change recipe to [{no}]: {recipeName}");
                Program.MainForm.Invoke(new MethodInvoker(delegate ()
                {
                    form.Close();
                }));
                return false;
            }

            var markerArg = new MarkerArgDefault();
            markerArg.Document = doc;
            markerArg.Rtc = seq.Rtc;
            markerArg.Laser = seq.Laser;
            markerArg.RtcListType = ListType.Auto;
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form.Percentage = 50;

                ////TRANS 레이어 섹제하고 ref 레이어 visible true
                //Layer transRightLayer = doc.Layers.NameOf("TRANS_RIGHT");
                //if (null != transRightLayer)
                //    doc.Action.ActEntityDelete(new List<IEntity>(transRightLayer));
                //var refLayerRightName = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_RIGHT");
                //var refLayerRight = doc.Layers.NameOf(refLayerRightName);
                //if (null != refLayerRight)
                //    refLayerRight.IsVisible = true;

                //Layer transLeftLayer = doc.Layers.NameOf("TRANS_LEFT");
                //if (null != transLeftLayer)
                //    doc.Action.ActEntityDelete(new List<IEntity>(transLeftLayer));
                //var refLayerLeftName = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_LEFT");
                //var refLayerLeft = doc.Layers.NameOf(refLayerLeftName);
                //if (null != refLayerLeft)
                //    refLayerLeft.IsVisible = true;

                form.Percentage = 70;

            }));
            success &= seq.Marker.Ready(markerArg);
            if (success)
            {
                Program.MainForm.Invoke(new MethodInvoker(delegate ()
                {
                    Application.DoEvents();

                    seq.Editor.Document = doc; //updated !
                    seq.Editor.FileName = recipeFileName;
                    seq.Viewer.FileName = recipeFileName;
                    //Viewer.Document = doc; 자동 !
                    Application.DoEvents();
                    Program.MainForm.Invoke(new MethodInvoker(delegate ()
                    {
                        form.Percentage = 100;
                    }));
                }));
                RecipeNo = no;
                RecipeName = recipeName;
                seq.Warn(WarnEnum.RecipeChanged);
                Logger.Log(Logger.Type.Warn, $"recipe has changed to [{no}]: {recipeName}");
            }
            else
            {
                // turn off ready status
                //seq.Editor.Document = null;
                seq.Marker.Clear();
                seq.Error(ErrEnum.RecipeChange);
                Logger.Log(Logger.Type.Warn, $"fail to change recipe to [{no}]: {recipeName}");
            }
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form.Close();
            }));
            return success;
        }
        public bool ReadScanFieldCorrectionInterval(out int rows, out int cols, out float rowInterval, out float colInterval)
        {
            rows = cols = 0;
            rowInterval = colInterval = 0;
            if (seq.isFieldCorrecting)
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"another field correction form is activating");
                return false;
            }
            string rootPath = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"FILE", "CORRECTION");
            string fileFullPath = Path.Combine(rootPath, $"scanner_matrix_gap_data.txt");
            if (false == File.Exists(fileFullPath))
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"try to open field correction file interval but failed : {fileFullPath}");
                return false;
            }

            string line = string.Empty;
            using (var stream = new StreamReader(fileFullPath))
            {
                while (!stream.EndOfStream)
                {
                    line = stream.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.StartsWith(";"))
                        continue;
                    //3,5,2,2
                    string[] tokens = line.Split(new char[] { ',', ';' });
                    if (4 != tokens.Length)
                    {
                        Logger.Log(Logger.Type.Error, $"invalid file format: {line} at {fileFullPath}");
                        return false;
                    }
                    rows = int.Parse(tokens[0]) ;
                    cols = int.Parse(tokens[1]);
                    rowInterval = float.Parse(tokens[2]);
                    colInterval = rowInterval;
                    //colInterval = float.Parse(tokens[3]);
                    return true;
                }
            }
            return false;
        }
        public bool ReadScannerFieldCorrection(string fileFullPath = "")
        {
            if (seq.isFieldCorrecting)
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"another field correction form is activating");
                return false;
            }
            //비전에서 기록한 보정 측정 정보
            if (string.IsNullOrEmpty(fileFullPath))
            {
                string rootPath = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"FILE", "CORRECTION");
                //scanner_calibration_5v5.txt
                fileFullPath = Path.Combine(rootPath, $"scanner_calibration_{this.FieldCorrectionRows}v{this.FieldCorrectionCols}.txt");
            }
            if (false == File.Exists(fileFullPath))
            {
                seq.Error(ErrEnum.VisionFieldCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"try to open field correction file but failed : {fileFullPath}");
                return false;
            }

            int rows = this.FieldCorrectionRows;
            int cols = this.FieldCorrectionCols;
            float rowInterval = this.FieldCorrectionRowInterval;
            float colInterval = this.FieldCorrectionColInterval;
            string sourceFile = seq.Rtc.CorrectionFiles[0];
            string targetFile = string.Empty;
            var correction2D = new RtcCorrection2D(seq.Rtc.KFactor, rows, cols, rowInterval, colInterval, sourceFile, targetFile);
            float left = -colInterval * (float)(int)(cols / 2);
            float top = rowInterval * (float)(int)(rows / 2);
            
            string line = string.Empty;
            var list = new List<Vector2>(rows * cols);
            using (var stream = new StreamReader(fileFullPath))
            {
                while (!stream.EndOfStream)
                {
                    line = stream.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.StartsWith(";"))
                        continue;
                    
                    string[] tokens = line.Split(new char[] { ',', ';' });
                    if (3 != tokens.Length)
                    {
                        Logger.Log(Logger.Type.Error, $"invalid file format: {line} at {fileFullPath}");
                        return false;
                    }
                    //[0], x, y
                    //int no = int.Parse(tokens[0]); 
                    float dx = float.Parse(tokens[1]);
                    float dy = float.Parse(tokens[2]);
                    list.Add(new Vector2(dx, dy));
                }
            }

            if (list.Count != rows*cols)
            {
                Logger.Log(Logger.Type.Error, $"field correcction data counts are mismatched. rows X cols= {this.FieldCorrectionRows} X {this.FieldCorrectionCols} but readed: {list.Count}");
                return false;
            }

            //180 rotate and reverse order
            list.Reverse();
            left = -colInterval * (float)(int)(cols / 2);
            top = rowInterval * (float)(int)(rows / 2);
            int index = 0;
            for (int row=0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    correction2D.AddRelative(row, col,
                        new Vector2(left + col * colInterval, top - row * rowInterval),
                        new Vector2(-list[index].X, -list[index].Y) // xy 비전 좌표값 반전
                        );
                    index++;
                }
            }

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
        public bool ReadZCorrection(out float dx, out float dy, out int counts)
        {
            dx = dy = counts = 0;
            string rootPath = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"FILE", "CORRECTION");
            //scanner_z_correct_data.txt
            string fileFullPath = Path.Combine(rootPath, $"scanner_z_correct_data.txt");
            if (false == File.Exists(fileFullPath))
            {
                seq.Error(ErrEnum.VisionZCorrectionOpen);
                Logger.Log(Logger.Type.Error, $"try to open z correction file but failed : {fileFullPath}");
                return false;
            }
            //scanner_x_gap,1
            //scanner_line_count,10
            //laser_z_pos,42
            //laser_z_gap,1
            //review_start_pos_x,30
            //review_start_pos_y,45
            string line = string.Empty;
            using (var stream = new StreamReader(fileFullPath))
            {
                while (!stream.EndOfStream)
                {
                    line = stream.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.StartsWith(";"))
                        continue;

                    string[] tokens = line.Split(new char[] { ',', ';' });
                    if (2 != tokens.Length)
                    {
                        Logger.Log(Logger.Type.Error, $"invalid file format: {line} at {fileFullPath}");
                        return false;
                    }
                    if (tokens[0] == "scanner_x_gap")
                    {
                        dx = 0;
                        dy = float.Parse(tokens[1]); // ______ 모양으로 가정됨
                    }
                    else if (tokens[0] == "scanner_line_count")
                    {
                        counts = int.Parse(tokens[1]);
                    }
                }
            }
            Logger.Log(Logger.Type.Debug, $"success to open scanner z correction: dx= {dx:F3}, dy= {dy:F3}, counts= {counts}");
            return true;
        }
        private void Form2D_OnApply(object sender, EventArgs e)
        {
            var form = sender as Correction2DForm;
            string ct5FileName = form.RtcCorrection.TargetCorrectionFile;
            if (!File.Exists(ct5FileName))
            {
                Logger.Log(Logger.Type.Error, $"try to change correction file but not exist : {ct5FileName}");
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
                Logger.Log(Logger.Type.Warn, $"correction file has changed to {ct5FileName} at {iniFileName}");
                var mb2 = new MessageBoxOk();
                mb2.ShowDialog("Correction", $"correction file has changed to {ct5FileName} at {iniFileName}");
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
        public bool PrepareDefectInEditor(int index, Group group, float refDx, float refDy, float refAngle)
        {
            if (seq.IsBusy)
            {
                Logger.Log(Logger.Type.Error, $"trying to prepare defect data into editor but busy");
                return false;
            }
            var doc = seq.Editor.Document; //에디터의 doc 를 대상으로
            Layer defLayer = null;
            var defLayerRight = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "DEFECT_RIGHT");
            var defLayerLeft = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "DEFECT_LEFT");
            switch (index)
            {
                case 1:
                    defLayer = doc.Layers.NameOf(defLayerRight);
                    break;
                case 2:
                    defLayer = doc.Layers.NameOf(defLayerLeft);
                    break;
                default:
                    Logger.Log(Logger.Type.Error, $"invalid left/right id: {index}");
                    return false;
            }
            if (null == defLayer || 0 == defLayer.Count)
            {
                seq.Error(ErrEnum.NoDefectLayer);
                Logger.Log(Logger.Type.Error, $"target layer name is not exist : {defLayerRight} or {defLayerLeft}");
                return false;
            }
            if (!(defLayer[0] is IPen))
            {
                seq.Error(ErrEnum.NoDefectLayer);
                Logger.Log(Logger.Type.Error, $"target layer is not start with pen entity");
                return false;
            }

            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                var form = new ProgressForm()
                {
                    Message = $"Creating Defect Data ..." + Environment.NewLine,
                    Percentage = 0,
                };
                form.Show(seq.Editor);
                Application.DoEvents();
                var deleteEntities = new List<IEntity>(defLayer.Count);
                foreach (var entity in defLayer)
                {
                    if (!(entity is IPen))
                        deleteEntities.Add(entity);
                }
                form.Percentage = 10;
                Application.DoEvents();
                doc.Action.ActEntityDelete(deleteEntities);
                Application.DoEvents();
                doc.Action.ActEntityAdd(group, defLayer);
                Application.DoEvents();
                form.Message += "Adding Polylines ..." + Environment.NewLine;
                doc.Action.SelectedEntity = null;
                form.Percentage = 50;
                Application.DoEvents();
                doc.Action.UndoRedoClear(); //100 undo 개수 제한이 있지만 메모리 소비가 클수있음 ? save memory
                form.Message += $"Regening ..." + Environment.NewLine;
                Application.DoEvents();
                seq.Editor.View.Render();                
                form.Percentage = 80;

                //form.Message += $"Transforming Reference Layers ..." + Environment.NewLine;
                //Application.DoEvents();

                //switch (index)
                //{
                //    case 1://right
                //        Layer transRightLayer = doc.Layers.NameOf("TRANS_RIGHT");
                //        if (null != transRightLayer)
                //            doc.Action.ActEntityDelete(new List<IEntity>(transRightLayer));
                //        var refLayerRightName = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_RIGHT");
                //        var refLayerRight = doc.Layers.NameOf(refLayerRightName);
                //        transRightLayer = refLayerRight.Clone() as Layer;
                //        transRightLayer.Name = "TRANS_RIGHT";
                //        transRightLayer.IsVisible = true;
                //        transRightLayer.Regen();
                //        transRightLayer.Rotate(refAngle, refLayerRight.BoundRect.Center);
                //        transRightLayer.Transit(new Vector2(refDx, refDy));
                //        doc.Action.ActEntityAdd(transRightLayer);
                //        refLayerRight.IsVisible = false;
                //        break;
                //    case 2://left
                //        Layer transLeftLayer = doc.Layers.NameOf("TRANS_LEFT");
                //        if (null != transLeftLayer)
                //            doc.Action.ActEntityDelete(new List<IEntity>(transLeftLayer));
                //        var refLayerLeftName = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_LEFT");
                //        var refLayerLeft = doc.Layers.NameOf(refLayerLeftName);
                //        transLeftLayer = refLayerLeft.Clone() as Layer;
                //        transLeftLayer.Name = "TRANS_LEFT";
                //        transLeftLayer.IsVisible = true;
                //        transLeftLayer.Regen();
                //        transLeftLayer.Rotate(refAngle, refLayerLeft.BoundRect.Center);
                //        transLeftLayer.Transit(new Vector2(refDx, refDy));
                //        doc.Action.ActEntityAdd(transLeftLayer);
                //        refLayerLeft.IsVisible = false;
                //        break;
                //}
                
                seq.Editor.View.Render();
                Application.DoEvents();
                form.Percentage = 100;
                Application.DoEvents();
                form.Close();
                Application.DoEvents();
            }));
            return true;
        }
        //public bool PrepareDefectInMarker(int index, Group group)
        //{
        //    // 오토 화면 
        //    //if (this.formMain.FormCurrent != this.formMain.FormAuto)
        //    //    return false;
        //    if (seq.IsBusy)
        //    {
        //        seq.Error(ErrEnum.Busy);
        //        Logger.Log(Logger.Type.Error, $"trying to change defect info into marker but busy");
        //        return false;
        //    }

        //    //var doc = seq.Marker.Document; //복제된 doc 를 대상으로 ??
        //    var doc = seq.Marker.Document; //에디터의  doc 를 대상으로 ?

        //    Layer layer = null;
        //    var defLayerRight = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "DEFECT_RIGHT");
        //    var defLayerLeft = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "DEFECT_LEFT");
        //    switch (index)
        //    {
        //        case 1:
        //            layer = doc.Layers.NameOf(defLayerRight);
        //            break;
        //        case 2:
        //            layer = doc.Layers.NameOf(defLayerLeft);
        //            break;
        //    }
        //    if (null == layer || 0 == layer.Count)
        //    {
        //        seq.Error(ErrEnum.NoDefectLayer);
        //        Logger.Log(Logger.Type.Error, $"target layer name is not exist : {defLayerRight} or {defLayerLeft}");
        //        return false;
        //    }
        //    if (!(layer[0] is IPen))
        //    {
        //        seq.Error(ErrEnum.NoDefectLayer);
        //        Logger.Log(Logger.Type.Error, $"target layer is not start with pen entity");
        //        return false;
        //    }

        //    //Program.MainForm.Invoke(new MethodInvoker(delegate ()
        //    //{
        //    // 첫번째 객체를 제외하고 모두 삭제
        //    var deleteEntities = new List<IEntity>(layer.Count);
        //        foreach (var entity in layer)
        //        {
        //            if (!(entity is IPen))
        //                deleteEntities.Add(entity);
        //        }
        //        doc.Action.ActEntityDelete(deleteEntities);
        //        doc.Action.ActEntityAdd(group, layer);
        //        doc.Action.UndoRedoClear(); //100 undo 개수 제한이 있지만 메모리 소비가 클수있음 ? save memory
        //    //}));
        //    return true;
        //}

        /// <summary>
        /// Defect 파일 열고 분석
        /// </summary>
        /// <param name="index">1(오른쪽), 2(왼쪽)</param>
        /// <param name="fileName">비전 검사 결과 파일 이름</param>
        /// <param name="group">그룹 객체들</param>
        /// <returns></returns>
        public bool ReadDefectFromFile(int index, string fileName, out Group group, out float refDx, out float refDy, out float refAngle)
        {

            //out List<Group> ??
            group = null;
            refDx = 0;
            refDy = 0;
            refAngle = 0;
            if (!File.Exists(fileName))
            {
                seq.Error(ErrEnum.VisionDefectDataOpen);
                Logger.Log(Logger.Type.Error, $"fail to open vision defect file : {fileName}");
                return false;
            }
            ProgressForm form = new ProgressForm();
            Program.MainForm.Invoke(new MethodInvoker(delegate ()
            {
                form.Message = $"Reading From Defect File : {fileName}";
                form.Percentage = 0;
                form.Show(seq.Editor);
            }));
            Application.DoEvents();

            var editor = seq.Editor;
            var doc = editor.Document; //에디터의 doc 를 대상
            bool isHatchable = false;
            HatchMode hatchMode = HatchMode.Line;
            float hatchAngle = 90;
            float hatch2Angle = 0;
            float hatchInterval = 0.1f;
            float hatchExclude = 0;
            uint repeat = 1;

            string extData = doc.ExtensionData;
            if (string.IsNullOrEmpty(extData))
            {
                Logger.Log(Logger.Type.Error, $"document extension data (hatch) is empty !");
            }
            else
            {
                var tempFileName = Path.GetTempFileName();
                using (StreamWriter sw = new StreamWriter(tempFileName))
                {
                    sw.Write(extData);
                }
                isHatchable = NativeMethods.ReadIni<bool>(tempFileName, $"HATCH", "HATCHABLE");
                hatchMode = (HatchMode)NativeMethods.ReadIni<int>(tempFileName, $"HATCH", "MODE");
                hatchAngle = NativeMethods.ReadIni<float>(tempFileName, $"HATCH", "ANGLE");
                hatch2Angle = NativeMethods.ReadIni<float>(tempFileName, $"HATCH", "ANGLE2");
                hatchInterval = NativeMethods.ReadIni<float>(tempFileName, $"HATCH", "INTERVAL");
                hatchExclude = NativeMethods.ReadIni<float>(tempFileName, $"HATCH", "EXCLUDE");
                repeat = NativeMethods.ReadIni<uint>(tempFileName, $"HATCH", "REPEAT");
                if (repeat <= 1)
                    repeat = 1;
                if (hatchInterval <= 0)
                    isHatchable = false;
            }

            bool success = true;
            group = new Group();
            group.Name = $"Defects";
            group.IsEnableFastRendering = true;
            group.IsHitTest = false; //선택 않되도록
            group.Align = Alignment.Center;
            group.Repeat = repeat;
            int polylineCount = 0;
            seq.Warn(WarnEnum.VisionDataOpening);

            var lineIndex = 0;
            var lineCount = File.ReadAllLines(fileName).Length;
            if (lineCount <= 0)
            {
                Program.MainForm.Invoke(new MethodInvoker(delegate ()
                {
                    form.Close();
                    Application.DoEvents();
                }));
                Logger.Log(Logger.Type.Error, $"no polyline data in {fileName}");
                return false;
            }
            try
            {
                using (var stream = new StreamReader(fileName))
                {
                    LwPolyline polyline = null;
                    while (!stream.EndOfStream)
                    {
                        string line = stream.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;
                        else if (line.StartsWith(";")) //주석
                            continue;
                        else if (line.StartsWith("POLYLINE_BEGIN"))
                        {
                            Debug.Assert(null == polyline);
                            string[] tokens = line.Split(',');
                            polyline = new LwPolyline();
                            if (tokens.Length > 1)
                                polyline.Name = tokens[1];
                        }
                        else if (line.StartsWith("POLYLINE_END"))
                        {
                            Debug.Assert(null != polyline);
                            if (polyline.Count >= 3)
                            {
                                polyline.IsClosed = true;
                                polyline.HatchMode = hatchMode;
                                polyline.IsHatchable = isHatchable;
                                polyline.HatchAngle = hatchAngle;
                                polyline.HatchAngle2 = hatch2Angle;
                                polyline.HatchInterval = hatchInterval;
                                polyline.HatchExclude = hatchExclude;
                            }
                            polyline.Regen();
                            group.Add(polyline);
                            polylineCount++;
                            polyline = null;
                        }
                        else
                        {
                            string[] tokens = line.Split(',');
                            float x = float.Parse(tokens[0]);
                            float y = float.Parse(tokens[1]);
                            polyline.Add(new LwPolyLineVertex(x, y));
                        }
     
                        float percentage = (float)lineIndex++ / (float)lineCount * 100.0f;
                        if (percentage % 10 == 0)
                        {
                            // 하나씩 혹은 한번에 전체를 ?
                            Program.MainForm.Invoke(new MethodInvoker(delegate ()
                            {
                                form.Percentage = percentage;
                                Application.DoEvents();
                            }));
                        }
                    }
                }
                Logger.Log(Logger.Type.Info, $"success to open defect file : {polylineCount} polylines at {fileName}");
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
                    Program.MainForm.Invoke(new MethodInvoker(delegate ()
                    {
                        form.Message = $"Regening {polylineCount} Polylines From {fileName}";
                        form.Percentage = 100;
                        Application.DoEvents();
                    }));
                    group.Regen();
                    //x 정렬 오름차순
                    //group.Sort(delegate (IEntity e1, IEntity e2)
                    //{
                    //    return e1.BoundRect.Center.X.CompareTo(e2.BoundRect.Center.X);
                    //});
                    switch (index)
                    {
                        case 1: //right
                            {
                                var refLayerRight = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_RIGHT");
                                var layer = doc.Layers.NameOf(refLayerRight);
                                if (null == layer)
                                {
                                    Logger.Log(Logger.Type.Error, $"target reference layer is not exist : {refLayerRight}");
                                }
                                else
                                {
                                    var br = layer.BoundRect;
                                    group.Transit(br.Center);
                                }
                            }
                            break;
                        case 2: //left
                            {
                                var refLayerLeft = NativeMethods.ReadIni<string>(FormMain.ConfigFileName, $"LAYER", "REF_LEFT");
                                var layer = doc.Layers.NameOf(refLayerLeft);
                                if (null == layer)
                                {
                                    Logger.Log(Logger.Type.Error, $"target reference layer is not exist : {refLayerLeft}");
                                }
                                else
                                {
                                    var br = layer.BoundRect;
                                    group.Transit(br.Center);
                                }
                            }
                            break;                             
                    }
                    //
                    seq.Warn(WarnEnum.VisionDataOpening, true);
                }
                Program.MainForm.Invoke(new MethodInvoker(delegate ()
                {
                    form.Close();
                    Application.DoEvents();
                }));
            }
            return success;
        }
    }
}