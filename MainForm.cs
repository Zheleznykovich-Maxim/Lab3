using System.Diagnostics;

namespace Lab3
{
    public partial class MainForm : Form
    {
        private Philosopher[] philosophers;
        private Mutex[] forks;
        private List<Panel> philosopherStatusPanels;
        private List<Panel> forksStatusPanels;
        private PictureBox[] philosopherPictureBoxes;
        private PictureBox[] forkPictureBoxes;
        private ListBox actionsListBox;
        private int ThinkingTime;
        private int EatingTime;
        private Stopwatch stopwatch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializePhilosophersAndForks()
        {
            this.actionsListBox = this.listBox1;
            this.philosophers = new Philosopher[5];
            this.forks = new Mutex[5];
            Random random = new Random();
            int result;
            if (int.TryParse(this.ThinkTime.Text, out result))
                this.ThinkingTime = result;
            if (int.TryParse(this.EatTime.Text, out result))
                this.EatingTime = result;
            this.philosopherStatusPanels = new List<Panel>()
            {
                panel1,
                panel2,
                panel3,
                panel4,
                panel5
            };

            this.forksStatusPanels = new List<Panel>()
            {
                p_fork1,
                p_fork2,
                p_fork3,
                p_fork4,
                p_fork5
            };

            this.philosopherPictureBoxes = new PictureBox[5]
            {
                phil1,
                phil2,
                phil3,
                phil4,
                phil5
            };
            this.forkPictureBoxes = new PictureBox[5]
            {
                fork1,
                fork2,
                fork3,
                fork4,
                fork5
            };
            for (int index = 0; index < 5; ++index)
                this.forks[index] = new Mutex();
            for (int index = 0; index < 5; ++index)
            {
                this.philosophers[index] = new Philosopher(index + 1, this.forks[index], this.forks[(index + 4) % 5], this.actionsListBox, this.EatingTime, this.ThinkingTime);
                this.philosophers[index].SetStatusPanel(this.philosopherStatusPanels[index]);
                this.philosophers[index].SetForkStatusPanel(this.forksStatusPanels[index], this.forksStatusPanels[(index + 4) % 5]);
                this.philosophers[index].SetPictureBox(this.philosopherPictureBoxes[index]);
                this.philosopherStatusPanels[index].BackColor = Color.Transparent;
                this.forksStatusPanels[index].BackColor = Color.Transparent;
                new Thread(new ThreadStart(this.philosophers[index].EatAndThink)).Start();
            }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            this.stopwatch.Start();
            this.InitializePhilosophersAndForks();
        }

        private void button_Pause_Click(object sender, EventArgs e)
        {
            foreach (Philosopher philosopher in this.philosophers)
            {
                philosopher.Stop();
            }
                
            stopwatch.Stop();
            MessageBox.Show($"Алгоритм проработал {stopwatch.ElapsedMilliseconds / 1000.0} секунд");
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(this.listBox1.Items[e.Index] is ColoredListBoxItem coloredListBoxItem))
                return;
            e.DrawBackground();
            using (SolidBrush solidBrush = new SolidBrush(coloredListBoxItem.Color))
                e.Graphics.DrawString(coloredListBoxItem.Text, e.Font, (Brush)solidBrush, (RectangleF)e.Bounds);
            e.DrawFocusRectangle();
        }
    }
}