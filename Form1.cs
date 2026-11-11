using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace _3DInterpretator
{
    public partial class Form1 : Form
    {
        // Извините за комментарии на двух разных языках.
        // Sorry for the comments in two different languages.


        // im recommend to NOT set MaxX,MaxY and MaxZ bigger value than 1000,
        // i read about this, "byte[,,] World" with size 1000x1000x1000 needs 1GB RAM
        // just dont do this if u dont have much ram, (my current pc have only 3GB)

        Random rnd = new Random();
        public const int MaxX = 50;
        public const int MaxY = 50;
        public const int MaxZ = 50;
        private byte[,,] World;
        private Bitmap _screen;
        private Cursor3D cockroach = new Cursor3D();
        private Stack<byte> Pockets;

        public Form1()
        {
            InitializeComponent();
            World = new byte[MaxX, MaxY, MaxZ];
            _screen = new Bitmap(MaxX, MaxY);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = _screen;
            trackBarZ.Maximum = MaxZ - 1;
            CRposition.Text = "X: None\nY: None\nZ:None";
            InputCode.Text = Properties.Settings.Default.PreviousUserCode;
        }

        private void trackBarZ_Scroll(object sender, EventArgs e)
        {
            RenderLayer(trackBarZ.Value);
        }

        class Cursor3D
        {
            public int X;
            public int Y;
            public int Z;
        }

        private void RenderLayer(int z)
        {
            CRposition.Text = $"\nX: {cockroach.X}\nY: {cockroach.Y}\nZ:{cockroach.Z}";
            if (z < 0 || z >= MaxZ) return;

            for (int y = 0; y < MaxY; y++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    // 1. Отрисовка курсора (он приоритетнее)
                    if (x == cockroach.X && y == cockroach.Y && z == cockroach.Z)
                    {
                        _screen.SetPixel(x, y, Color.Red);
                        continue; // Переходим к следующей ячейке, чтобы не перекрасить курсор
                    }

                    // 2. Отрисовка значения ячейки (монохром)
                    byte val = World[x, y, z];
                    Color cellColor = Color.FromArgb(val, val, val);

                    // Если val == 0 — будет чисто черный (Color.Black)
                    // Если val == 255 — будет чисто белый (Color.White)

                    _screen.SetPixel(x, y, cellColor);
                }
            }

            // Говорим PictureBox'у, что картинка обновилась
            pictureBox1.Refresh();
        }

        private void CodeTransform_Click(object sender, EventArgs e)
        {
            World = new byte[MaxX, MaxY, MaxZ];
            cockroach.X = 0;
            cockroach.Y = 0;
            cockroach.Z = 0;
            Pockets = new Stack<byte>();
            Dictionary<string, int> flags = new Dictionary<string, int>();

            int RotationHorizontal = 0;
            int RotationVertical = 0;

            int instructionCount = 0;
            const int MaxInstructions = 1_000_000;

            bool IfCondition = false;

            int LineCount = 0;

            StringBuilder OutPut = new StringBuilder();

            string CR = "Null";

            string MLR = "";  // for future debugging

            try
            {
                string[] Code = InputCode.Lines;
                while (LineCount < Code.Length)
                {
                    instructionCount++;
                    if (instructionCount > MaxInstructions)
                    {
                        OutPut.Append("\n[ERROR] Infinite loop detected! Execution aborted.\n");
                        break;
                    }

                    string Line = Code[LineCount];
                loopstart:;
                    string[] NewLine = Line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (NewLine.Length == 0 || Line.Substring(0,2) == "//") { goto NextLine; } // Пропускаем пустые строки и комментарии
                    switch (NewLine[0].ToLower())
                    {
                        case "<":
                            RotationHorizontal--;
                            if (RotationHorizontal < 0) { RotationHorizontal = 3; }
                            goto NextLine;
                        case ">":
                            RotationHorizontal++;
                            if (RotationHorizontal > 3) { RotationHorizontal = 0; }
                            goto NextLine;
                        case "^":
                            if (RotationVertical <= 0) { RotationVertical++; }
                            goto NextLine;
                        case "v":
                            if (RotationVertical >= 0) { RotationVertical--; }
                            goto NextLine;

                        case "bring":
                            byte bit = World[cockroach.X, cockroach.Y, cockroach.Z];
                            Pockets.Push(bit);
                            World[cockroach.X, cockroach.Y, cockroach.Z] = 0;
                            goto NextLine;

                        case "chpk":
                            OutPut.Append($"current Pocket = {Pockets.Peek()}\n");
                            goto NextLine;

                        case "ppop":
                            Pockets.Pop();
                            goto NextLine;

                        case "drop":
                            World[cockroach.X, cockroach.Y, cockroach.Z] = Pockets.Peek();
                            Pockets.Pop();
                            goto NextLine;

                        case "padd":
                            World[cockroach.X, cockroach.Y, cockroach.Z] += Pockets.Peek();
                            Pockets.Pop();
                            goto NextLine;

                        case "psub":
                            World[cockroach.X, cockroach.Y, cockroach.Z] -= Pockets.Peek();
                            Pockets.Pop();
                            goto NextLine;

                        case "pdup":
                            Pockets.Push(Pockets.Peek());
                            goto NextLine;

                        case "pswap":
                            if (Pockets.Count >= 2)
                            {
                                byte top = Pockets.Pop();
                                byte second = Pockets.Pop();
                                Pockets.Push(top);
                                Pockets.Push(second);
                            }
                            goto NextLine;

                        case "rand":
                            World[cockroach.X, cockroach.Y, cockroach.Z] = (byte)rnd.Next(0, 256);
                            goto NextLine;

                        case "pand":
                            World[cockroach.X, cockroach.Y, cockroach.Z] &= Pockets.Pop();
                            goto NextLine;
                        case "por":
                            World[cockroach.X, cockroach.Y, cockroach.Z] |= Pockets.Pop();
                            goto NextLine;

                        case "crd": // debugging
                            OutPut.Append(CR + '\n');
                            break;

                        case "cpd": //debugging
                            string CP = "X: " + cockroach.X.ToString() + " | " + "Y: " + cockroach.Y.ToString() + " | " + "Z: " + cockroach.Z.ToString();
                            OutPut.Append(CP + '\n');
                            break;

                        case "set":
                            if (NewLine[1] == "p")
                            {
                                World[cockroach.X, cockroach.Y, cockroach.Z] = Pockets.Peek(); // Упадет, если пусто, но мы же любим fail-fast!
                            }
                            else
                            {
                                byte SetCellTry;
                                if (byte.TryParse(NewLine[1], out SetCellTry))
                                {
                                    World[cockroach.X, cockroach.Y, cockroach.Z] = SetCellTry;
                                }
                                else
                                {
                                    OutPut.Append("can’t set the cell value\n");
                                }
                            }
                            goto NextLine;

                        case "get":
                            OutPut.Append($"Cell {cockroach.X.ToString() +"|"+ cockroach.Y.ToString() + "|" + cockroach.Z.ToString()} :  {World[cockroach.X, cockroach.Y, cockroach.Z]}\n");
                            goto NextLine;

                        case "flag":
                            flags.Add(NewLine[1], LineCount);
                            goto NextLine;
                        case "jump":
                            LineCount = flags[NewLine[1]];
                            goto NextLine;

                        case "sct":
                            IfCondition = true;
                            goto NextLine;

                        case "scf":
                            IfCondition = false;
                            goto NextLine;

                        case "decr":
                            World[cockroach.X, cockroach.Y, cockroach.Z]--;
                            goto NextLine;
                        case "incr":
                            World[cockroach.X, cockroach.Y, cockroach.Z]++;
                            goto NextLine;

                        case "ifcell":
                            switch (NewLine[1])
                            {
                                case "==":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] == byte.Parse(NewLine[2])) {IfCondition = true;} else { IfCondition = false; }
                                    break;
                                case ">":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] > byte.Parse(NewLine[2])) { IfCondition = true; } else { IfCondition = false; }
                                    break;
                                case ">=":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] >= byte.Parse(NewLine[2])) { IfCondition = true; } else { IfCondition = false; }
                                    break;
                                case "<":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] < byte.Parse(NewLine[2])) { IfCondition = true; } else { IfCondition = false; }
                                    break;
                                case "<=":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] <= byte.Parse(NewLine[2])) { IfCondition = true; } else { IfCondition = false; }
                                    break;
                                case "!=":
                                    if (World[cockroach.X, cockroach.Y, cockroach.Z] != byte.Parse(NewLine[2])) { IfCondition = true; } else { IfCondition = false; }
                                    break;
                            }
                            goto NextLine;

                        case "tcon":
                            if (IfCondition)
                            {
                                Line = Line.Substring(4).Trim();
                                goto loopstart;
                            }
                            else { goto NextLine; }
                        case "fcon":
                            if (!IfCondition)
                            {
                                Line = Line.Substring(4).Trim();
                                goto loopstart;
                            }
                            else { goto NextLine; }


                        case "move":
                            // Vertical movement is a priority.
                            int steps = 1;
                            if (NewLine.Length > 1 && int.TryParse(NewLine[1], out int parsedSteps))
                            {
                                steps = parsedSteps;
                            }
                            switch (RotationVertical)
                            {
                                case 0:
                                    switch (RotationHorizontal)
                                    {
                                        case 0:
                                            MLR = "X+";
                                            cockroach.X = (cockroach.X + steps) % MaxX;
                                            break;
                                        case 1:
                                            MLR = "Y-";
                                            cockroach.Y = (cockroach.Y - (steps % MaxY) + MaxY) % MaxY;
                                            break;
                                        case 2:
                                            MLR = "X-";
                                            cockroach.X = (cockroach.X - (steps % MaxX) + MaxX) % MaxX;
                                            break;
                                        case 3:
                                            MLR = "Y+";
                                            cockroach.Y = (cockroach.Y + steps) % MaxY;
                                            break;
                                    }
                                    break;
                                case 1:
                                    MLR = "Z+";
                                    cockroach.Z = (cockroach.Z + steps) % MaxZ;
                                    break;
                                case -1:
                                    MLR = "Z-";
                                    cockroach.Z = (cockroach.Z - (steps % MaxZ) + MaxZ) % MaxZ;
                                    break;
                            }
                            CR = MLR;
                            goto NextLine;

                    }

                NextLine:;
                    LineCount++;
                }
            }
            catch (Exception ex)
            {
                OutPut.Append(ex);
            }

            OutPutConsole.Text = OutPut.ToString();
            RenderLayer(trackBarZ.Value);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Отключаем сглаживание
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // Убираем размытие по краям пикселей (сдвиг на пол-пикселя)
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            // Отключаем сглаживание краев
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // Рисуем нашу картинку, растягивая её на весь PictureBox
            e.Graphics.DrawImage(_screen, pictureBox1.ClientRectangle);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PreviousUserCode = InputCode.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Cockroach Script (*.cwp)|*.cwp|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                sfd.DefaultExt = "cwp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(sfd.FileName, InputCode.Text);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Cockroach Script (*.cwp)|*.cwp|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    InputCode.Text = System.IO.File.ReadAllText(ofd.FileName);
                }
            }
        }
    }
}