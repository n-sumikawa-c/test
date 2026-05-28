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
        private WinForms.CheckBox chkWholeDrawing;
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
            this.Text = "雲マーク自動作成";
            this.Width = 420;
            this.Height = 370;
            this.StartPosition = WinForms.FormStartPosition.CenterScreen;
            this.TopMost = false;

            chkBlockAlignedGroup = new WinForms.CheckBox();
            chkBlockAlignedGroup.Text = "ブロック参照：位置合わせグループ";
            chkBlockAlignedGroup.Left = 20;
            chkBlockAlignedGroup.Top = 20;
            chkBlockAlignedGroup.Width = 360;
            chkBlockAlignedGroup.Checked = true;

            chkBlockEach = new WinForms.CheckBox();
            chkBlockEach.Text = "ブロック参照：個別";
            chkBlockEach.Left = 20;
            chkBlockEach.Top = 50;
            chkBlockEach.Width = 360;
            chkBlockEach.Checked = false;

            chkWholeDrawing = new WinForms.CheckBox();
            chkWholeDrawing.Text = "図面全体";
            chkWholeDrawing.Left = 20;
            chkWholeDrawing.Top = 80;
            chkWholeDrawing.Width = 360;
            chkWholeDrawing.Checked = false;

            chkVisibleLayerOnly = new WinForms.CheckBox();
            chkVisibleLayerOnly.Text = "表示中の画層のみ対象";
            chkVisibleLayerOnly.Left = 20;
            chkVisibleLayerOnly.Top = 110;
            chkVisibleLayerOnly.Width = 360;
            chkVisibleLayerOnly.Checked = true;

            chkDeleteExistingClouds = new WinForms.CheckBox();
            chkDeleteExistingClouds.Text = "既存雲マークを削除してから作成";
            chkDeleteExistingClouds.Left = 20;
            chkDeleteExistingClouds.Top = 140;
            chkDeleteExistingClouds.Width = 360;
            chkDeleteExistingClouds.Checked = true;

            WinForms.Label lblOffset = new WinForms.Label();
            lblOffset.Text = "オフセット量";
            lblOffset.Left = 20;
            lblOffset.Top = 185;
            lblOffset.Width = 150;

            numOffset = new WinForms.NumericUpDown();
            numOffset.Left = 190;
            numOffset.Top = 180;
            numOffset.Width = 120;
            numOffset.Minimum = 0;
            numOffset.Maximum = 1000000;
            numOffset.Value = 100;
            numOffset.DecimalPlaces = 0;

            WinForms.Label lblArcLength = new WinForms.Label();
            lblArcLength.Text = "雲マーク山ピッチ";
            lblArcLength.Left = 20;
            lblArcLength.Top = 215;
            lblArcLength.Width = 150;

            numArcLength = new WinForms.NumericUpDown();
            numArcLength.Left = 190;
            numArcLength.Top = 210;
            numArcLength.Width = 120;
            numArcLength.Minimum = 1;
            numArcLength.Maximum = 1000000;
            numArcLength.Value = 200;
            numArcLength.DecimalPlaces = 0;

            WinForms.Label lblAlignTolerance = new WinForms.Label();
            lblAlignTolerance.Text = "位置合わせ許容";
            lblAlignTolerance.Left = 20;
            lblAlignTolerance.Top = 245;
            lblAlignTolerance.Width = 150;

            numAlignTolerance = new WinForms.NumericUpDown();
            numAlignTolerance.Left = 190;
            numAlignTolerance.Top = 240;
            numAlignTolerance.Width = 120;
            numAlignTolerance.Minimum = 0;
            numAlignTolerance.Maximum = 1000000;
            numAlignTolerance.Value = 10;
            numAlignTolerance.DecimalPlaces = 0;

            btnCreate = new WinForms.Button();
            btnCreate.Text = "作成";
            btnCreate.Left = 90;
            btnCreate.Top = 295;
            btnCreate.Width = 90;
            btnCreate.Click += BtnCreate_Click;

            btnClose = new WinForms.Button();
            btnClose.Text = "閉じる";
            btnClose.Left = 220;
            btnClose.Top = 295;
            btnClose.Width = 90;
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(chkBlockAlignedGroup);
            this.Controls.Add(chkBlockEach);
            this.Controls.Add(chkWholeDrawing);
            this.Controls.Add(chkVisibleLayerOnly);
            this.Controls.Add(chkDeleteExistingClouds);

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

            bool blockAlignedGroup = chkBlockAlignedGroup.Checked;
            bool blockEach = chkBlockEach.Checked;
            bool wholeDrawing = chkWholeDrawing.Checked;
            bool visibleLayerOnly = chkVisibleLayerOnly.Checked;
            bool deleteExistingClouds = chkDeleteExistingClouds.Checked;

            double offset = Convert.ToDouble(numOffset.Value);
            double arcLength = Convert.ToDouble(numArcLength.Value);
            double alignTolerance = Convert.ToDouble(numAlignTolerance.Value);

            if (!blockAlignedGroup && !blockEach && !wholeDrawing)
            {
                WinForms.MessageBox.Show("処理対象を1つ以上選択してください。");
                return;
            }

            int createdCount = 0;
            int deletedCount = 0;

            try
            {
                using (DocumentLock lkdoc = doc.LockDocument())
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    string layerName = DateTime.Now.ToString("yyyyMMdd") + "_check";

                    ObjectId layerId = EnsureLayer(db, tr, layerName);
                    EnsureRegApp(db, tr, CheckCloudConst.RegAppName);

                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    if (bt == null)
                    {
                        WinForms.MessageBox.Show("BlockTableを取得できませんでした。");
                        return;
                    }

                    BlockTableRecord modelSpace =
                        tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if (modelSpace == null)
                    {
                        WinForms.MessageBox.Show("ModelSpaceを取得できませんでした。");
                        return;
                    }

                    if (deleteExistingClouds)
                    {
                        deletedCount = DeleteExistingCheckClouds(tr, modelSpace, layerName);
                    }

                    List<Extents3d> blockExtents = GetBlockReferenceExtents(
                        tr,
                        modelSpace,
                        visibleLayerOnly
                    );

                    if (blockAlignedGroup)
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

                    if (blockEach)
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

                    if (wholeDrawing)
                    {
                        Extents3d? ext = GetModelSpaceExtents(tr, modelSpace, visibleLayerOnly);

                        if (ext.HasValue)
                        {
                            Polyline cloud = CreateRectangularCloud(ext.Value, offset, arcLength);
                            cloud.LayerId = layerId;
                            SetCheckCloudXData(cloud);

                            modelSpace.AppendEntity(cloud);
                            tr.AddNewlyCreatedDBObject(cloud, true);

                            createdCount++;
                        }
                    }

                    tr.Commit();
                }

                ed.WriteMessage($"\n既存雲マーク削除数: {deletedCount}");
                ed.WriteMessage($"\n雲マーク作成数: {createdCount}");
                ed.WriteMessage("\nCHECKCLOUD 完了。");

                WinForms.MessageBox.Show(
                    "処理が完了しました。\n\n" +
                    "削除数：" + deletedCount + "\n" +
                    "作成数：" + createdCount,
                    "雲マーク自動作成"
                );
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("\nエラー: " + ex.Message);
                WinForms.MessageBox.Show(ex.Message, "エラー");
            }
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

        private static List<Extents3d> GetBlockReferenceExtents(
            Transaction tr,
            BlockTableRecord modelSpace,
            bool visibleLayerOnly)
        {
            List<Extents3d> result = new List<Extents3d>();

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

                if (IsCheckLayer(ent.Layer))
                {
                    continue;
                }

                BlockReference br = ent as BlockReference;

                if (br == null)
                {
                    continue;
                }

                if (visibleLayerOnly)
                {
                    if (!IsEntityLayerVisible(tr, br))
                    {
                        continue;
                    }

                    Extents3d? visibleExt = GetVisibleBlockReferenceExtents(tr, br);

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
                        // GeometricExtents が取得できないブロック参照はスキップ
                    }
                }
            }

            return result;
        }

        private static Extents3d? GetVisibleBlockReferenceExtents(
            Transaction tr,
            BlockReference br)
        {
            if (br == null)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, br))
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

                    if (!IsEntityLayerVisible(tr, childEnt))
                    {
                        continue;
                    }

                    Extents3d? childExt = GetEntityExtentsWithTransforms(tr, childEnt, transforms);

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

                // 属性文字がある場合も含める
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

                    if (!IsEntityLayerVisible(tr, att))
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
                        // 属性のExtents取得不可はスキップ
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
    List<Matrix3d> transforms)
        {
            if (ent == null)
            {
                return null;
            }

            if (ent.IsErased)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, ent))
            {
                return null;
            }

            // ブロック内にさらにブロックがある場合
            BlockReference nestedBr = ent as BlockReference;

            if (nestedBr != null)
            {
                return GetNestedBlockReferenceExtents(tr, nestedBr, transforms);
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
    List<Matrix3d> parentTransforms)
        {
            if (nestedBr == null)
            {
                return null;
            }

            if (!IsEntityLayerVisible(tr, nestedBr))
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

                    if (!IsEntityLayerVisible(tr, childEnt))
                    {
                        continue;
                    }

                    Extents3d? childExt = GetEntityExtentsWithTransforms(tr, childEnt, transforms);

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


        private static Extents3d? GetModelSpaceExtents(
            Transaction tr,
            BlockTableRecord modelSpace,
            bool visibleLayerOnly)
        {
            Extents3d? total = null;

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

                if (IsCheckLayer(ent.Layer))
                {
                    continue;
                }

                if (visibleLayerOnly)
                {
                    if (!IsEntityLayerVisible(tr, ent))
                    {
                        continue;
                    }
                }

                try
                {
                    Extents3d? ext = null;

                    BlockReference br = ent as BlockReference;

                    if (visibleLayerOnly && br != null)
                    {
                        ext = GetVisibleBlockReferenceExtents(tr, br);
                    }
                    else
                    {
                        ext = ent.GeometricExtents;
                    }

                    if (ext.HasValue)
                    {
                        if (!total.HasValue)
                        {
                            total = ext.Value;
                        }
                        else
                        {
                            Extents3d merged = total.Value;
                            merged.AddExtents(ext.Value);
                            total = merged;
                        }
                    }
                }
                catch
                {
                    // Extents が取得できない図形はスキップ
                }
            }

            return total;
        }

        private static bool IsEntityLayerVisible(Transaction tr, Entity ent)
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

            LayerTableRecord ltr =
                tr.GetObject(layerId, OpenMode.ForRead) as LayerTableRecord;

            if (ltr == null)
            {
                return true;
            }

            if (ltr.IsOff)
            {
                return false;
            }

            if (ltr.IsFrozen)
            {
                return false;
            }

            return true;
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

            if (blockExtents == null)
            {
                return result;
            }

            if (blockExtents.Count == 0)
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