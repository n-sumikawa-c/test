using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Windows.Forms;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

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

    public class CheckCloudForm : Form
    {
        private CheckBox chkBlockAlignedGroup;
        private CheckBox chkBlockEach;
        private CheckBox chkWholeDrawing;
        private CheckBox chkDeleteExistingClouds;

        private NumericUpDown numOffset;
        private NumericUpDown numArcLength;
        private NumericUpDown numAlignTolerance;

        private Button btnCreate;
        private Button btnClose;

        public CheckCloudForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "雲マーク自動作成";
            this.Width = 380;
            this.Height = 330;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            chkBlockAlignedGroup = new CheckBox();
            chkBlockAlignedGroup.Text = "ブロック参照：位置合わせグループ";
            chkBlockAlignedGroup.Left = 20;
            chkBlockAlignedGroup.Top = 20;
            chkBlockAlignedGroup.Width = 320;
            chkBlockAlignedGroup.Checked = true;

            chkBlockEach = new CheckBox();
            chkBlockEach.Text = "ブロック参照：個別";
            chkBlockEach.Left = 20;
            chkBlockEach.Top = 50;
            chkBlockEach.Width = 320;
            chkBlockEach.Checked = false;

            chkWholeDrawing = new CheckBox();
            chkWholeDrawing.Text = "図面全体";
            chkWholeDrawing.Left = 20;
            chkWholeDrawing.Top = 80;
            chkWholeDrawing.Width = 320;
            chkWholeDrawing.Checked = false;

            chkDeleteExistingClouds = new CheckBox();
            chkDeleteExistingClouds.Text = "既存雲マークを削除してから作成";
            chkDeleteExistingClouds.Left = 20;
            chkDeleteExistingClouds.Top = 110;
            chkDeleteExistingClouds.Width = 320;
            chkDeleteExistingClouds.Checked = true;

            System.Windows.Forms.Label lblOffset = new System.Windows.Forms.Label();
            lblOffset.Text = "オフセット量";
            lblOffset.Left = 20;
            lblOffset.Top = 150;
            lblOffset.Width = 130;

            numOffset = new NumericUpDown();
            numOffset.Left = 170;
            numOffset.Top = 145;
            numOffset.Width = 120;
            numOffset.Minimum = 0;
            numOffset.Maximum = 1000000;
            numOffset.Value = 100;
            numOffset.DecimalPlaces = 0;

            System.Windows.Forms.Label lblArcLength = new System.Windows.Forms.Label();
            lblArcLength.Text = "雲マーク山ピッチ";
            lblArcLength.Left = 20;
            lblArcLength.Top = 180;
            lblArcLength.Width = 130;

            numArcLength = new NumericUpDown();
            numArcLength.Left = 170;
            numArcLength.Top = 175;
            numArcLength.Width = 120;
            numArcLength.Minimum = 1;
            numArcLength.Maximum = 1000000;
            numArcLength.Value = 200;
            numArcLength.DecimalPlaces = 0;

            System.Windows.Forms.Label lblAlignTolerance = new System.Windows.Forms.Label();
            lblAlignTolerance.Text = "位置合わせ許容距離";
            lblAlignTolerance.Left = 20;
            lblAlignTolerance.Top = 210;
            lblAlignTolerance.Width = 140;

            numAlignTolerance = new NumericUpDown();
            numAlignTolerance.Left = 170;
            numAlignTolerance.Top = 205;
            numAlignTolerance.Width = 120;
            numAlignTolerance.Minimum = 0;
            numAlignTolerance.Maximum = 1000000;
            numAlignTolerance.Value = 10;
            numAlignTolerance.DecimalPlaces = 0;

            btnCreate = new Button();
            btnCreate.Text = "作成";
            btnCreate.Left = 70;
            btnCreate.Top = 250;
            btnCreate.Width = 90;
            btnCreate.Click += BtnCreate_Click;

            btnClose = new Button();
            btnClose.Text = "閉じる";
            btnClose.Left = 190;
            btnClose.Top = 250;
            btnClose.Width = 90;
            btnClose.Click += BtnClose_Click;

            this.Controls.Add(chkBlockAlignedGroup);
            this.Controls.Add(chkBlockEach);
            this.Controls.Add(chkWholeDrawing);
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
                MessageBox.Show("アクティブな図面がありません。");
                return;
            }

            Editor ed = doc.Editor;
            Database db = doc.Database;

            bool blockAlignedGroup = chkBlockAlignedGroup.Checked;
            bool blockEach = chkBlockEach.Checked;
            bool wholeDrawing = chkWholeDrawing.Checked;
            bool deleteExistingClouds = chkDeleteExistingClouds.Checked;

            double offset = Convert.ToDouble(numOffset.Value);
            double arcLength = Convert.ToDouble(numArcLength.Value);
            double alignTolerance = Convert.ToDouble(numAlignTolerance.Value);

            if (!blockAlignedGroup && !blockEach && !wholeDrawing)
            {
                MessageBox.Show("処理対象を1つ以上選択してください。");
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
                        MessageBox.Show("BlockTableを取得できませんでした。");
                        return;
                    }

                    BlockTableRecord modelSpace =
                        tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    if (modelSpace == null)
                    {
                        MessageBox.Show("ModelSpaceを取得できませんでした。");
                        return;
                    }

                    if (deleteExistingClouds)
                    {
                        deletedCount = DeleteExistingCheckClouds(tr, modelSpace, layerName);
                    }

                    List<Extents3d> blockExtents = GetBlockReferenceExtents(tr, modelSpace);

                    if (blockAlignedGroup)
                    {
                        List<Extents3d> groups =
                            CreateAlignedBlockGroups(blockExtents, alignTolerance);

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
                        Extents3d? ext = GetModelSpaceExtents(tr, modelSpace);

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

                MessageBox.Show(
                    $"処理が完了しました。\n\n削除数：{deletedCount}\n作成数：{createdCount}",
                    "雲マーク自動作成");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("\nエラー: " + ex.Message);
                MessageBox.Show(ex.Message, "エラー");
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

        private static bool IsCheckCloudEntity(Entity ent)
        {
            if (ent == null)
            {
                return false;
            }

            ResultBuffer rb = ent.GetXDataForApplication(CheckCloudConst.RegAppName);

            if (rb == null)
            {
                return false;
            }

            TypedValue[] values = rb.AsArray();

            foreach (TypedValue tv in values)
            {
                if (tv.TypeCode == (int)DxfCode.ExtendedDataAsciiString)
                {
                    string value = tv.Value as string;

                    if (value == CheckCloudConst.XDataValue)
                    {
                        return true;
                    }
                }
            }

            return false;
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

                /*
                 * このツールで作成した雲マークはXDataを持つ。
                 * ただし、初回導入前に作った同日_checkレイヤ上のPolylineも消したい場合があるため、
                 * 同じ日付_checkレイヤ上のPolylineは削除対象にしている。
                 */
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
            BlockTableRecord modelSpace)
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

                try
                {
                    Extents3d ext = br.GeometricExtents;
                    result.Add(ext);
                }
                catch
                {
                    // GeometricExtents が取得できないブロックはスキップ
                }
            }

            return result;
        }

        private static Extents3d? GetModelSpaceExtents(
            Transaction tr,
            BlockTableRecord modelSpace)
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

                try
                {
                    Extents3d ext = ent.GeometricExtents;

                    if (!total.HasValue)
                    {
                        total = ext;
                    }
                    else
                    {
                        Extents3d merged = total.Value;
                        merged.AddExtents(ext);
                        total = merged;
                    }
                }
                catch
                {
                    // Extentsが取れない図形はスキップ
                }
            }

            return total;
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
            pl.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

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