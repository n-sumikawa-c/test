using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using WinForms = System.Windows.Forms;

namespace TestLib
{
    public class Class1
    {
        [CommandMethod("CHECKCLOUD")]
        public static void CheckCloud()
        {
            CheckCloudForm form = new CheckCloudForm();
            AcAp.ShowModelessDialog(form);
        }
    }

    public class CheckCloudForm : WinForms.Form
    {
        private WinForms.CheckBox chkBlockAlignedGroup;
        private WinForms.CheckBox chkBlockEach;
        private WinForms.CheckBox chkBlockAll;

        private WinForms.RadioButton rdoTargetWholeDrawing;
        private WinForms.RadioButton rdoTargetWindowSelection;
        private WinForms.RadioButton rdoTargetObjectSelection;

        private WinForms.CheckBox chkVisibleLayerOnly;
        private WinForms.CheckBox chkDeleteExistingClouds;

        private WinForms.NumericUpDown numOffset;
        private WinForms.NumericUpDown numArcLength;
        private WinForms.NumericUpDown numAlignTolerance;

        private WinForms.Button btnCreate;
        private WinForms.Button btnClose;

        public CheckCloudForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "AutoCloud ver4";
            this.Width = 460;
            this.Height = 560;
            this.StartPosition = WinForms.FormStartPosition.CenterScreen;
            this.TopMost = false;

            WinForms.GroupBox grpProcess = new WinForms.GroupBox();
            grpProcess.Text = "処理内容";
            grpProcess.Left = 20;
            grpProcess.Top = 20;
            grpProcess.Width = 400;
            grpProcess.Height = 120;

            chkBlockAlignedGroup = new WinForms.CheckBox();
            chkBlockAlignedGroup.Text = "ブロック参照：位置合わせ";
            chkBlockAlignedGroup.Left = 20;
            chkBlockAlignedGroup.Top = 25;
            chkBlockAlignedGroup.Width = 330;
            chkBlockAlignedGroup.Checked = true;

            chkBlockEach = new WinForms.CheckBox();
            chkBlockEach.Text = "ブロック参照：個別";
            chkBlockEach.Left = 20;
            chkBlockEach.Top = 55;
            chkBlockEach.Width = 330;
            chkBlockEach.Checked = false;

            chkBlockAll = new WinForms.CheckBox();
            chkBlockAll.Text = "ブロック参照：全体";
            chkBlockAll.Left = 20;
            chkBlockAll.Top = 85;
            chkBlockAll.Width = 330;
            chkBlockAll.Checked = false;

            grpProcess.Controls.Add(chkBlockAlignedGroup);
            grpProcess.Controls.Add(chkBlockEach);
            grpProcess.Controls.Add(chkBlockAll);

            WinForms.GroupBox grpTarget = new WinForms.GroupBox();
            grpTarget.Text = "対象物";
            grpTarget.Left = 20;
            grpTarget.Top = 155;
            grpTarget.Width = 400;
            grpTarget.Height = 120;

            rdoTargetWholeDrawing = new WinForms.RadioButton();
            rdoTargetWholeDrawing.Text = "図面全体";
            rdoTargetWholeDrawing.Left = 20;
            rdoTargetWholeDrawing.Top = 25;
            rdoTargetWholeDrawing.Width = 330;
            rdoTargetWholeDrawing.Checked = true;

            rdoTargetWindowSelection = new WinForms.RadioButton();
            rdoTargetWindowSelection.Text = "範囲選択をしたブロックのみ対象";
            rdoTargetWindowSelection.Left = 20;
            rdoTargetWindowSelection.Top = 55;
            rdoTargetWindowSelection.Width = 330;
            rdoTargetWindowSelection.Checked = false;

            rdoTargetObjectSelection = new WinForms.RadioButton();
            rdoTargetObjectSelection.Text = "選択したブロック参照のみ対象";
            rdoTargetObjectSelection.Left = 20;
            rdoTargetObjectSelection.Top = 85;
            rdoTargetObjectSelection.Width = 330;
            rdoTargetObjectSelection.Checked = false;

            grpTarget.Controls.Add(rdoTargetWholeDrawing);
            grpTarget.Controls.Add(rdoTargetWindowSelection);
            grpTarget.Controls.Add(rdoTargetObjectSelection);

            WinForms.GroupBox grpOption = new WinForms.GroupBox();
            grpOption.Text = "オプション";
            grpOption.Left = 20;
            grpOption.Top = 290;
            grpOption.Width = 400;
            grpOption.Height = 90;

            chkVisibleLayerOnly = new WinForms.CheckBox();
            chkVisibleLayerOnly.Text = "表示中の画層のみ対象";
            chkVisibleLayerOnly.Left = 20;
            chkVisibleLayerOnly.Top = 25;
            chkVisibleLayerOnly.Width = 330;
            chkVisibleLayerOnly.Checked = true;

            chkDeleteExistingClouds = new WinForms.CheckBox();
            chkDeleteExistingClouds.Text = "既存雲マークを削除してから作成";
            chkDeleteExistingClouds.Left = 20;
            chkDeleteExistingClouds.Top = 55;
            chkDeleteExistingClouds.Width = 330;
            chkDeleteExistingClouds.Checked = true;

            grpOption.Controls.Add(chkVisibleLayerOnly);
            grpOption.Controls.Add(chkDeleteExistingClouds);

            WinForms.Label lblOffset = new WinForms.Label();
            lblOffset.Text = "オフセット量";
            lblOffset.Left = 40;
            lblOffset.Top = 405;
            lblOffset.Width = 150;

            numOffset = new WinForms.NumericUpDown();
            numOffset.Left = 210;
            numOffset.Top = 400;
            numOffset.Width = 120;
            numOffset.Minimum = 0;
            numOffset.Maximum = 1000000;
            numOffset.Value = 100;
            numOffset.DecimalPlaces = 0;

            WinForms.Label lblArcLength = new WinForms.Label();
            lblArcLength.Text = "雲マーク山ピッチ";
            lblArcLength.Left = 40;
            lblArcLength.Top = 435;
            lblArcLength.Width = 150;

            numArcLength = new WinForms.NumericUpDown();
            numArcLength.Left = 210;
            numArcLength.Top = 430;
            numArcLength.Width = 120;
            numArcLength.Minimum = 1;
            numArcLength.Maximum = 1000000;
            numArcLength.Value = 200;
            numArcLength.DecimalPlaces = 0;

            WinForms.Label lblAlignTolerance = new WinForms.Label();
            lblAlignTolerance.Text = "位置合わせ許容";
            lblAlignTolerance.Left = 40;
            lblAlignTolerance.Top = 465;
            lblAlignTolerance.Width = 150;

            numAlignTolerance = new WinForms.NumericUpDown();
            numAlignTolerance.Left = 210;
            numAlignTolerance.Top = 460;
            numAlignTolerance.Width = 120;
            numAlignTolerance.Minimum = 0;
            numAlignTolerance.Maximum = 1000000;
            numAlignTolerance.Value = 10;
            numAlignTolerance.DecimalPlaces = 0;

            btnCreate = new WinForms.Button();
            btnCreate.Text = "作成";
            btnCreate.Left = 100;
            btnCreate.Top = 500;
            btnCreate.Width = 100;
            btnCreate.Click += BtnCreate_Click;

            btnClose = new WinForms.Button();
            btnClose.Text = "閉じる";
            btnClose.Left = 230;
            btnClose.Top = 500;
            btnClose.Width = 100;
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(grpProcess);
            this.Controls.Add(grpTarget);
            this.Controls.Add(grpOption);

            this.Controls.Add(lblOffset);
            this.Controls.Add(numOffset);

            this.Controls.Add(lblArcLength);
            this.Controls.Add(numArcLength);

            this.Controls.Add(lblAlignTolerance);
            this.Controls.Add(numAlignTolerance);

            this.Controls.Add(btnCreate);
            this.Controls.Add(btnClose);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            Document doc = AcAp.DocumentManager.MdiActiveDocument;

            if (doc == null)
            {
                WinForms.MessageBox.Show("アクティブな図面がありません。");
                return;
            }

            Editor ed = doc.Editor;
            Database db = doc.Database;

            bool processAlignedGroup = chkBlockAlignedGroup.Checked;
            bool processEach = chkBlockEach.Checked;
            bool processBlockAll = chkBlockAll.Checked;

            bool targetWindowSelection = rdoTargetWindowSelection.Checked;
            bool targetObjectSelection = rdoTargetObjectSelection.Checked;

            bool visibleLayerOnly = chkVisibleLayerOnly.Checked;
            bool deleteExistingClouds = chkDeleteExistingClouds.Checked;

            double offset = Convert.ToDouble(numOffset.Value);
            double arcLength = Convert.ToDouble(numArcLength.Value);
            double alignTolerance = Convert.ToDouble(numAlignTolerance.Value);

            if (!processAlignedGroup && !processEach && !processBlockAll)
            {
                WinForms.MessageBox.Show("処理内容を1つ以上選択してください。");
                return;
            }

            int createdCount = 0;
            int deletedCount = 0;

            bool formHidden = false;

            try
            {
                List<ObjectId> selectedBlockIds = null;

                if (targetWindowSelection)
                {
                    this.Hide();
                    formHidden = true;

                    selectedBlockIds = PromptBlockSelectionByWindow(ed);

                    ClearAutoCadSelectionPrompt(ed, false);
                    ScheduleAutoCadPromptClear(ed);

                    if (selectedBlockIds == null || selectedBlockIds.Count == 0)
                    {
                        RestoreFormIfHidden(formHidden);

                        ed.WriteMessage("\n範囲内にブロック参照が見つからなかったため、処理を中止しました。");
                        WinForms.MessageBox.Show("範囲内にブロック参照が見つかりませんでした。");
                        return;
                    }
                }
                else if (targetObjectSelection)
                {
                    this.Hide();
                    formHidden = true;

                    selectedBlockIds = PromptBlockSelectionByObjects(ed);

                    ClearAutoCadSelectionPrompt(ed, false);
                    ScheduleAutoCadPromptClear(ed);

                    if (selectedBlockIds == null || selectedBlockIds.Count == 0)
                    {
                        RestoreFormIfHidden(formHidden);

                        ed.WriteMessage("\nブロック参照が選択されなかったため、処理を中止しました。");
                        WinForms.MessageBox.Show("ブロック参照が選択されませんでした。");
                        return;
                    }
                }

                using (DocumentLock lkdoc = doc.LockDocument())
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    Dictionary<ObjectId, bool> layerVisibleCache = new Dictionary<ObjectId, bool>();

                    string layerName = DateTime.Now.ToString("yyyyMMdd") + "_check";

                    ObjectId layerId = EnsureLayer(db, tr, layerName);
                    EnsureRegApp(db, tr, CheckCloudConst.RegAppName);

                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    if (bt == null)
                    {
                        RestoreFormIfHidden(formHidden);
                        WinForms.MessageBox.Show("BlockTableを取得できませんでした。");
                        return;
                    }

                    BlockTableRecord modelSpace =
                        tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if (modelSpace == null)
                    {
                        RestoreFormIfHidden(formHidden);
                        WinForms.MessageBox.Show("ModelSpaceを取得できませんでした。");
                        return;
                    }

                    if (deleteExistingClouds)
                    {
                        deletedCount = DeleteExistingCheckClouds(tr, modelSpace, layerName);
                    }

                    List<Extents3d> blockExtents = GetTargetBlockReferenceExtents(
                        tr,
                        modelSpace,
                        visibleLayerOnly,
                        targetWindowSelection,
                        targetObjectSelection,
                        selectedBlockIds,
                        layerVisibleCache
                    );

                    if (blockExtents.Count == 0)
                    {
                        ed.WriteMessage("\n対象ブロック参照がありません。");
                        tr.Commit();

                        RestoreFormIfHidden(formHidden);
                        WinForms.MessageBox.Show("対象ブロック参照がありません。");
                        return;
                    }

                    if (processAlignedGroup)
                    {
                        List<Extents3d> groups = CreateAlignedBlockGroups(blockExtents, alignTolerance);

                        foreach (Extents3d ext in groups)
                        {
                            Polyline cloud = CreateRectangularCloud(ext, offset, arcLength);
                            cloud.LayerId = layerId;
                            SetCheckCloudXData(cloud);

                            modelSpace.AppendEntity(cloud);
                            tr.AddNewlyCreatedDBObject(cloud, true);

                            createdCount++;
                        }
                    }

                    if (processEach)
                    {
                        foreach (Extents3d ext in blockExtents)
                        {
                            Polyline cloud = CreateRectangularCloud(ext, offset, arcLength);
                            cloud.LayerId = layerId;
                            SetCheckCloudXData(cloud);

                            modelSpace.AppendEntity(cloud);
                            tr.AddNewlyCreatedDBObject(cloud, true);

                            createdCount++;
                        }
                    }

                    if (processBlockAll)
                    {
                        Extents3d? merged = MergeExtents(blockExtents);

                        if (merged.HasValue)
                        {
                            Polyline cloud = CreateRectangularCloud(merged.Value, offset, arcLength);
                            cloud.LayerId = layerId;
                            SetCheckCloudXData(cloud);

                            modelSpace.AppendEntity(cloud);
                            tr.AddNewlyCreatedDBObject(cloud, true);

                            createdCount++;
                        }
                    }

                    tr.Commit();
                }

                ClearAutoCadSelectionPrompt(ed, false);
                ScheduleAutoCadPromptClear(ed);

                RestoreFormIfHidden(formHidden);

                ed.WriteMessage($"\n既存雲マーク削除数: {deletedCount}");
                ed.WriteMessage($"\n雲マーク作成数: {createdCount}");
                ed.WriteMessage("\nCHECKCLOUD ver4 完了。");

                WinForms.MessageBox.Show(
                    "処理が完了しました。\n\n" +
                    "削除数：" + deletedCount + "\n" +
                    "作成数：" + createdCount,
                    "AutoCloud ver4"
                );
            }
            catch (System.Exception ex)
            {
                ClearAutoCadSelectionPrompt(ed, false);
                ScheduleAutoCadPromptClear(ed);

                RestoreFormIfHidden(formHidden);

                ed.WriteMessage("\nエラー: " + ex.Message);
                WinForms.MessageBox.Show(ex.Message, "エラー");
            }
        }

        private void RestoreFormIfHidden(bool formHidden)
        {
            if (formHidden && !this.Visible)
            {
                this.Show();
                this.Activate();
            }
        }

        private static void ClearAutoCadSelectionPrompt(Editor ed, bool forceRegen)
        {
            if (ed == null)
            {
                return;
            }

            try
            {
                ed.SetImpliedSelection(new ObjectId[0]);
            }
            catch
            {
            }

            try
            {
                ed.UpdateScreen();
            }
            catch
            {
            }

            if (forceRegen)
            {
                try
                {
                    ed.Regen();
                }
                catch
                {
                }
            }
        }

        private static void ScheduleAutoCadPromptClear(Editor ed)
        {
            ScheduleAutoCadPromptClear(ed, 200, false);
            ScheduleAutoCadPromptClear(ed, 700, false);

            // それでも右クリック確定後の表示残りが消えない場合だけ、
            // 下の行を有効化してください。Regenは少し重いです。
            // ScheduleAutoCadPromptClear(ed, 1200, true);
        }

        private static void ScheduleAutoCadPromptClear(Editor ed, int intervalMilliseconds, bool forceRegen)
        {
            if (ed == null)
            {
                return;
            }

            WinForms.Timer timer = new WinForms.Timer();
            timer.Interval = intervalMilliseconds;

            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                timer.Dispose();

                try
                {
                    ClearAutoCadSelectionPrompt(ed, forceRegen);
                }
                catch
                {
                }
            };

            timer.Start();
        }

        private static List<ObjectId> PromptBlockSelectionByWindow(Editor ed)
        {
            List<ObjectId> ids = new List<ObjectId>();
            HashSet<ObjectId> idSet = new HashSet<ObjectId>();

            PromptPointOptions ppopt1 = new PromptPointOptions("\n範囲選択 1点目を指定してください: ");
            PromptPointResult pt1 = ed.GetPoint(ppopt1);

            if (pt1.Status != PromptStatus.OK)
            {
                ClearAutoCadSelectionPrompt(ed, false);
                ScheduleAutoCadPromptClear(ed);
                return ids;
            }

            PromptCornerOptions ppopt2 = new PromptCornerOptions(
                "\n範囲選択 2点目を指定してください: ",
                pt1.Value
            );

            PromptPointResult pt2 = ed.GetCorner(ppopt2);

            if (pt2.Status != PromptStatus.OK)
            {
                ClearAutoCadSelectionPrompt(ed, false);
                ScheduleAutoCadPromptClear(ed);
                return ids;
            }

            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT")
            };

            SelectionFilter filter = new SelectionFilter(tvs);

            PromptSelectionResult res = ed.SelectCrossingWindow(pt1.Value, pt2.Value, filter);

            ClearAutoCadSelectionPrompt(ed, false);
            ScheduleAutoCadPromptClear(ed);

            if (res.Status != PromptStatus.OK)
            {
                return ids;
            }

            SelectionSet ss = res.Value;

            if (ss == null)
            {
                return ids;
            }

            foreach (SelectedObject selObj in ss)
            {
                if (selObj == null)
                {
                    continue;
                }

                if (selObj.ObjectId.IsNull)
                {
                    continue;
                }

                if (idSet.Add(selObj.ObjectId))
                {
                    ids.Add(selObj.ObjectId);
                }
            }

            return ids;
        }

        private static List<ObjectId> PromptBlockSelectionByObjects(Editor ed)
        {
            List<ObjectId> ids = new List<ObjectId>();
            HashSet<ObjectId> idSet = new HashSet<ObjectId>();

            ed.WriteMessage("\n対象にするブロック参照を選択してください。Enterまたは右クリックで確定してください。");

            TypedValue[] tvs = new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT")
            };

            SelectionFilter filter = new SelectionFilter(tvs);

            /*
             * PromptSelectionOptions.MessageForAdding を使うと、
             * 右クリック確定時にダイナミック入力の吹き出しが残る場合がある。
             * そのため、ここでは GetSelection(filter) のみを使う。
             */
            PromptSelectionResult res = ed.GetSelection(filter);

            ClearAutoCadSelectionPrompt(ed, false);
            ScheduleAutoCadPromptClear(ed);

            if (res.Status != PromptStatus.OK)
            {
                return ids;
            }

            SelectionSet ss = res.Value;

            if (ss == null)
            {
                return ids;
            }

            foreach (SelectedObject selObj in ss)
            {
                if (selObj == null)
                {
                    continue;
                }

                if (selObj.ObjectId.IsNull)
                {
                    continue;
                }

                if (idSet.Add(selObj.ObjectId))
                {
                    ids.Add(selObj.ObjectId);
                }
            }

            ClearAutoCadSelectionPrompt(ed, false);
            ScheduleAutoCadPromptClear(ed);

            return ids;
        }

        private static ObjectId EnsureLayer(Database db, Transaction tr, string layerName)
        {
            LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

            if (lt == null)
            {
                return ObjectId.Null;
            }

            if (lt.Has(layerName))
            {
                return lt[layerName];
            }

            lt.UpgradeOpen();

            LayerTableRecord ltr = new LayerTableRecord();
            ltr.Name = layerName;
            ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);

            ObjectId layerId = lt.Add(ltr);
            tr.AddNewlyCreatedDBObject(ltr, true);

            return layerId;
        }

        private static void EnsureRegApp(Database db, Transaction tr, string regAppName)
        {
            RegAppTable rat =
                tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;

            if (rat == null)
            {
                return;
            }

            if (rat.Has(regAppName))
            {
                return;
            }

            rat.UpgradeOpen();

            RegAppTableRecord record = new RegAppTableRecord();
            record.Name = regAppName;

            rat.Add(record);
            tr.AddNewlyCreatedDBObject(record, true);
        }

        private static void SetCheckCloudXData(Entity ent)
        {
            ResultBuffer rb = new ResultBuffer(
                new TypedValue((int)DxfCode.ExtendedDataRegAppName, CheckCloudConst.RegAppName),
                new TypedValue((int)DxfCode.ExtendedDataAsciiString, CheckCloudConst.XDataValue)
            );

            ent.XData = rb;
        }

        private static int DeleteExistingCheckClouds(
            Transaction tr,
            BlockTableRecord modelSpace,
            string layerName)
        {
            int deletedCount = 0;
            List<ObjectId> eraseIds = new List<ObjectId>();

            foreach (ObjectId id in modelSpace)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;

                if (ent == null)
                {
                    continue;
                }

                if (ent.IsErased)
                {
                    continue;
                }

                if (!string.Equals(ent.Layer, layerName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!(ent is Polyline))
                {
                    continue;
                }

                eraseIds.Add(id);
            }

            foreach (ObjectId id in eraseIds)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;

                if (ent == null)
                {
                    continue;
                }

                if (ent.IsErased)
                {
                    continue;
                }

                ent.Erase();
                deletedCount++;
            }

            return deletedCount;
        }

        private static List<Extents3d> GetTargetBlockReferenceExtents(
            Transaction tr,
            BlockTableRecord modelSpace,
            bool visibleLayerOnly,
            bool targetWindowSelection,
            bool targetObjectSelection,
            List<ObjectId> selectedBlockIds,
            Dictionary<ObjectId, bool> layerVisibleCache)
        {
            List<Extents3d> result = new List<Extents3d>();
            List<ObjectId> targetIds = new List<ObjectId>();

            if ((targetWindowSelection || targetObjectSelection) && selectedBlockIds != null)
            {
                foreach (ObjectId id in selectedBlockIds)
                {
                    targetIds.Add(id);
                }
            }
            else
            {
                foreach (ObjectId id in modelSpace)
                {
                    targetIds.Add(id);
                }
            }

            foreach (ObjectId id in targetIds)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;

                if (ent == null)
                {
                    continue;
                }

                if (ent.IsErased)
                {
                    continue;
                }

                if (IsCheckLayer(ent.Layer))
                {
                    continue;
                }

                BlockReference br = ent as BlockReference;

                if (br == null)
                {
                    continue;
                }

                if (br.OwnerId != modelSpace.ObjectId)
                {
                    continue;
                }

                if (visibleLayerOnly)
                {
                    if (!IsEntityLayerVisible(tr, br, layerVisibleCache))
                    {
                        continue;
                    }

                    Extents3d? visibleExt = GetVisibleBlockReferenceExtents(tr, br, layerVisibleCache);

                    if (visibleExt.HasValue)
                    {
                        result.Add(visibleExt.Value);
                    }
                }
                else
                {
                    try
                    {
                        Extents3d ext = br.GeometricExtents;
                        result.Add(ext);
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        private static Extents3d? MergeExtents(List<Extents3d> extents)
        {
            if (extents == null || extents.Count == 0)
            {
                return null;
            }

            Extents3d merged = extents[0];

            for (int i = 1; i < extents.Count; i++)
            {
                merged.AddExtents(extents[i]);
            }

            return merged;
        }

        private static Extents3d? GetVisibleBlockReferenceExtents(
            Transaction tr,
            BlockReference br,
            Dictionary<ObjectId, bool> layerVisibleCache)
        {
            if (br == null)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, br, layerVisibleCache))
            {
                return null;
            }

            Extents3d? total = null;

            try
            {
                BlockTableRecord btr =
                    tr.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                if (btr == null)
                {
                    return null;
                }

                List<Matrix3d> transforms = new List<Matrix3d>();
                transforms.Add(br.BlockTransform);

                foreach (ObjectId id in btr)
                {
                    Entity childEnt = tr.GetObject(id, OpenMode.ForRead) as Entity;

                    if (childEnt == null)
                    {
                        continue;
                    }

                    if (childEnt.IsErased)
                    {
                        continue;
                    }

                    if (IsCheckLayer(childEnt.Layer))
                    {
                        continue;
                    }

                    if (!IsEntityLayerVisible(tr, childEnt, layerVisibleCache))
                    {
                        continue;
                    }

                    Extents3d? childExt = GetEntityExtentsWithTransforms(
                        tr,
                        childEnt,
                        transforms,
                        layerVisibleCache
                    );

                    if (childExt.HasValue)
                    {
                        if (!total.HasValue)
                        {
                            total = childExt.Value;
                        }
                        else
                        {
                            Extents3d merged = total.Value;
                            merged.AddExtents(childExt.Value);
                            total = merged;
                        }
                    }
                }

                foreach (ObjectId attId in br.AttributeCollection)
                {
                    AttributeReference att =
                        tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;

                    if (att == null)
                    {
                        continue;
                    }

                    if (att.IsErased)
                    {
                        continue;
                    }

                    if (!IsEntityLayerVisible(tr, att, layerVisibleCache))
                    {
                        continue;
                    }

                    try
                    {
                        Extents3d attExt = att.GeometricExtents;

                        if (!total.HasValue)
                        {
                            total = attExt;
                        }
                        else
                        {
                            Extents3d merged = total.Value;
                            merged.AddExtents(attExt);
                            total = merged;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                return null;
            }

            return total;
        }

        private static Extents3d? GetEntityExtentsWithTransforms(
            Transaction tr,
            Entity ent,
            List<Matrix3d> transforms,
            Dictionary<ObjectId, bool> layerVisibleCache)
        {
            if (ent == null)
            {
                return null;
            }

            if (ent.IsErased)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, ent, layerVisibleCache))
            {
                return null;
            }

            BlockReference nestedBr = ent as BlockReference;

            if (nestedBr != null)
            {
                return GetNestedBlockReferenceExtents(
                    tr,
                    nestedBr,
                    transforms,
                    layerVisibleCache
                );
            }

            Entity clonedEnt = null;

            try
            {
                clonedEnt = ent.Clone() as Entity;

                if (clonedEnt == null)
                {
                    return null;
                }

                foreach (Matrix3d mat in transforms)
                {
                    clonedEnt.TransformBy(mat);
                }

                Extents3d ext = clonedEnt.GeometricExtents;

                return ext;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (clonedEnt != null)
                {
                    clonedEnt.Dispose();
                }
            }
        }

        private static Extents3d? GetNestedBlockReferenceExtents(
            Transaction tr,
            BlockReference nestedBr,
            List<Matrix3d> parentTransforms,
            Dictionary<ObjectId, bool> layerVisibleCache)
        {
            if (nestedBr == null)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, nestedBr, layerVisibleCache))
            {
                return null;
            }

            Extents3d? total = null;

            try
            {
                BlockTableRecord btr =
                    tr.GetObject(nestedBr.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                if (btr == null)
                {
                    return null;
                }

                List<Matrix3d> transforms = new List<Matrix3d>();

                transforms.Add(nestedBr.BlockTransform);

                foreach (Matrix3d mat in parentTransforms)
                {
                    transforms.Add(mat);
                }

                foreach (ObjectId id in btr)
                {
                    Entity childEnt = tr.GetObject(id, OpenMode.ForRead) as Entity;

                    if (childEnt == null)
                    {
                        continue;
                    }

                    if (childEnt.IsErased)
                    {
                        continue;
                    }

                    if (IsCheckLayer(childEnt.Layer))
                    {
                        continue;
                    }

                    if (!IsEntityLayerVisible(tr, childEnt, layerVisibleCache))
                    {
                        continue;
                    }

                    Extents3d? childExt = GetEntityExtentsWithTransforms(
                        tr,
                        childEnt,
                        transforms,
                        layerVisibleCache
                    );

                    if (childExt.HasValue)
                    {
                        if (!total.HasValue)
                        {
                            total = childExt.Value;
                        }
                        else
                        {
                            Extents3d merged = total.Value;
                            merged.AddExtents(childExt.Value);
                            total = merged;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return total;
        }

        private static bool IsEntityLayerVisible(
            Transaction tr,
            Entity ent,
            Dictionary<ObjectId, bool> layerVisibleCache)
        {
            if (ent == null)
            {
                return false;
            }

            ObjectId layerId = ent.LayerId;

            if (layerId.IsNull)
            {
                return true;
            }

            if (layerVisibleCache != null && layerVisibleCache.ContainsKey(layerId))
            {
                return layerVisibleCache[layerId];
            }

            bool visible = true;

            try
            {
                LayerTableRecord ltr =
                    tr.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;

                if (ltr != null)
                {
                    if (ltr.IsOff || ltr.IsFrozen)
                    {
                        visible = false;
                    }
                }
            }
            catch
            {
                visible = true;
            }

            if (layerVisibleCache != null && !layerVisibleCache.ContainsKey(layerId))
            {
                layerVisibleCache.Add(layerId, visible);
            }

            return visible;
        }

        private static bool IsCheckLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                return false;
            }

            return layerName.EndsWith("_check", StringComparison.OrdinalIgnoreCase);
        }

        private static List<Extents3d> CreateAlignedBlockGroups(
            List<Extents3d> blockExtents,
            double tolerance)
        {
            List<Extents3d> result = new List<Extents3d>();

            if (blockExtents == null || blockExtents.Count == 0)
            {
                return result;
            }

            int count = blockExtents.Count;
            bool[] visited = new bool[count];

            for (int i = 0; i < count; i++)
            {
                if (visited[i])
                {
                    continue;
                }

                Queue<int> queue = new Queue<int>();
                queue.Enqueue(i);
                visited[i] = true;

                Extents3d groupExt = blockExtents[i];

                while (queue.Count > 0)
                {
                    int current = queue.Dequeue();

                    groupExt.AddExtents(blockExtents[current]);

                    for (int j = 0; j < count; j++)
                    {
                        if (visited[j])
                        {
                            continue;
                        }

                        if (AreExtentsConnected(blockExtents[current], blockExtents[j], tolerance))
                        {
                            visited[j] = true;
                            queue.Enqueue(j);
                        }
                    }
                }

                result.Add(groupExt);
            }

            return result;
        }

        private static bool AreExtentsConnected(
            Extents3d a,
            Extents3d b,
            double tolerance)
        {
            double aMinX = a.MinPoint.X;
            double aMinY = a.MinPoint.Y;
            double aMaxX = a.MaxPoint.X;
            double aMaxY = a.MaxPoint.Y;

            double bMinX = b.MinPoint.X;
            double bMinY = b.MinPoint.Y;
            double bMaxX = b.MaxPoint.X;
            double bMaxY = b.MaxPoint.Y;

            double dx = 0.0;

            if (aMaxX < bMinX)
            {
                dx = bMinX - aMaxX;
            }
            else if (bMaxX < aMinX)
            {
                dx = aMinX - bMaxX;
            }

            double dy = 0.0;

            if (aMaxY < bMinY)
            {
                dy = bMinY - aMaxY;
            }
            else if (bMaxY < aMinY)
            {
                dy = aMinY - bMaxY;
            }

            double distance = Math.Sqrt(dx * dx + dy * dy);

            return distance <= tolerance;
        }

        private static Polyline CreateRectangularCloud(
            Extents3d ext,
            double offset,
            double arcLength)
        {
            double minX = ext.MinPoint.X - offset;
            double minY = ext.MinPoint.Y - offset;
            double maxX = ext.MaxPoint.X + offset;
            double maxY = ext.MaxPoint.Y + offset;

            if (arcLength <= 0)
            {
                arcLength = 100;
            }

            List<Point2d> points = new List<Point2d>();

            AddEdgePoints(points, new Point2d(minX, minY), new Point2d(maxX, minY), arcLength);
            AddEdgePoints(points, new Point2d(maxX, minY), new Point2d(maxX, maxY), arcLength);
            AddEdgePoints(points, new Point2d(maxX, maxY), new Point2d(minX, maxY), arcLength);
            AddEdgePoints(points, new Point2d(minX, maxY), new Point2d(minX, minY), arcLength);

            Polyline pl = new Polyline();

            double bulge = 0.5;

            for (int i = 0; i < points.Count; i++)
            {
                pl.AddVertexAt(i, points[i], bulge, 0, 0);
            }

            pl.Closed = true;

            return pl;
        }

        private static void AddEdgePoints(
            List<Point2d> points,
            Point2d start,
            Point2d end,
            double segmentLength)
        {
            Vector2d vec = end - start;
            double length = vec.Length;

            if (length <= 0)
            {
                return;
            }

            int segmentCount = Math.Max(1, (int)Math.Ceiling(length / segmentLength));

            for (int i = 0; i < segmentCount; i++)
            {
                double t = (double)i / (double)segmentCount;

                Point2d p = new Point2d(
                    start.X + vec.X * t,
                    start.Y + vec.Y * t
                );

                points.Add(p);
            }
        }
    }

    public static class CheckCloudConst
    {
        public const string RegAppName = "CHECKCLOUD_TOOL";
        public const string XDataValue = "CHECKCLOUD_ENTITY";
    }
}
