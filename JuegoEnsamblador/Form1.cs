using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JuegoEnsamblador
{

    public partial class Juego : Form
    {
        SolidBrush l_brush = new SolidBrush(Color.Gray);
        SolidBrush p_brush = new SolidBrush(Color.Yellow);
        SolidBrush b_brush = new SolidBrush(Color.White);
        Jugador jugador;
        Bola bola;
        List<Ladrillo> ladrillos = new List<Ladrillo>();
        public int vidas = 3, puntos = 0;
        public Juego()
        {
            InitializeComponent();
            jugador = new Jugador(gameField);
            bola = new Bola(gameField);
            ladrillos = Ladrillo.fabricaLadrillos(35, gameField);
            
        }
        private void Juego_KeyDown(object sender, KeyEventArgs e)
        {
            jugador.darDireccion(e.KeyCode);
        }
        private void Juego_KeyUp(object sender, KeyEventArgs e)
        {
            jugador.quitarDireccion(e.KeyCode);
        }

        private void gameField_Paint(object sender, PaintEventArgs e)
        {
            jugador.dibujar(p_brush, e.Graphics);
            bola.dibujar(b_brush, e.Graphics);
            foreach (Ladrillo l in ladrillos)
            {
                l.dibujar(l_brush, e.Graphics);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            List<Ladrillo> removed = new List<Ladrillo>();
            foreach (Ladrillo l in ladrillos)
            {
                if (bola.colision(l.x, l.y, l.w, l.h, false))
                {
                    removed.Add(l);
                    break;
                }
            }
            foreach (Ladrillo l in removed)
            {
                ladrillos.Remove(l);
                puntos += 100;
            }
            bola.colision(jugador.x, jugador.y, jugador.w, jugador.h, true);
            bola.mover(gameField, this);
            jugador.mover(gameField.Width);

            lblPuntos.Text = "Puntos: " + puntos;
            lblVidas.Text = "Vidas: " + vidas;
            if (vidas == 0)
            {
                timer1.Stop();
                MessageBox.Show("Perdiste");
                timer1.Start();
                jugador = new Jugador(gameField);
                bola = new Bola(gameField);
                ladrillos = Ladrillo.fabricaLadrillos(35, gameField);
                vidas = 3;
                puntos = 0;
            }
            if (ladrillos.Count == 0)
            {
                timer1.Stop();
                MessageBox.Show("Ganaste");
                timer1.Start();
            }
            this.Refresh();
        }
        
    }
    public class Ladrillo
    {
        public int x, y, w, h;
        public Color color;
        private static Random rnd = new Random();
        public Ladrillo(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            color = Color.FromArgb(rnd.Next(80,256), rnd.Next(80,256), rnd.Next(80,256));
        }

        public static List<Ladrillo> fabricaLadrillos(int n, PictureBox gameField)
        {
            List<Ladrillo> ladrillos = new List<Ladrillo>();
            int l_w = gameField.Width / (n/5);
            for (int i = 0; i < n; i++)
            {
                Ladrillo l = new Ladrillo((i % 7) * l_w, i / 7 * 30, l_w, 30);
                ladrillos.Add(l);
            }
            return ladrillos;
        }

        public void dibujar(SolidBrush brush, Graphics g)
        {
            brush.Color = color;
            g.FillRectangle(brush, x, y, w, h);
        }
    }

    public class Jugador
    {
        public int x, y, w = 80, h = 20, velocidad = 20;
        bool moverIzq, moverDer;

        public Jugador(PictureBox gameField)
        {
            x = (gameField.Width - w) / 2;
            y = gameField.Height - h - 10;
        }
        public void darDireccion(Keys key)
        {
            if (key == Keys.Left)
            {
                moverIzq = true;
            }
            else if (key == Keys.Right)
            {
                moverDer = true;
            }
        }
        public void quitarDireccion(Keys key)
        {
            if (key == Keys.Left)
            {
                moverIzq = false;
            }
            else if (key == Keys.Right)
            {
                moverDer = false;  
            }
        }
        public void mover(int gameWidth)
        {
            if (moverIzq && moverDer)
            {
                return;
            }
            if (moverIzq)
            {
                x -= velocidad;
            }
            else if(moverDer)
            {
                x += velocidad;
            }
            if (x < 0)
            {
                x = 0;
            }
            else if (x + w > gameWidth)
            {
                x = gameWidth - w;
            }
        }
        public void dibujar(SolidBrush brush, Graphics g)
        {
            g.FillRectangle(brush, x, y, w, h);
        }
    }

    public class Bola
    {
        int x, y, w = 20, h = 20;
        float velocidad_x, velocidad_y;
        private static Random rnd = new Random();

        public Bola(PictureBox gameField)
        {
            x = (gameField.Width - w) / 2;
            y = (int)(gameField.Height * .75);
            velocidadAlAzar();
        }

        public void velocidadAlAzar()
        {
            int normal = rnd.Next(100);
            float angulo = normal / 100f * (float)(Math.PI / 2) + (float)(Math.PI / 4);
            velocidad_x = 10 * -(float)Math.Cos(angulo);
            velocidad_y = 10 * -(float)Math.Sin(angulo);
        }

        public void mover(PictureBox gameField, Juego juego)
        {
            x += (int)velocidad_x;
            y += (int)velocidad_y;
            if (x < 0)
            {
                velocidad_x *= -1;
                x = 0;
            }
            else if (x + w > gameField.Width)
            {
                velocidad_x *= -1;
                x = gameField.Width - w;
            }
            if (y < 0)
            {
                velocidad_y *= -1;
                y = 0;
            }
            else if (y > gameField.Width)
            {
                x = (gameField.Width - w) / 2;
                y = (int)(gameField.Height * .75);
                juego.vidas--;
                velocidadAlAzar();
            }
        }

        public void dibujar(SolidBrush brush, Graphics g)
        {
            g.FillEllipse(brush, x, y, w, h);
        }

        public bool colision(int x2, int y2, int w2, int h2, bool azarArriba)
        {
            Rectangle r_bola = new Rectangle(x, y, w, h);
            Rectangle r_otro_arriba = new Rectangle(x2, y2, w2, 1);
            Rectangle r_otro_abajo = new Rectangle(x2, y2+h2-1, w2, 1);
            Rectangle r_otro_izq = new Rectangle(x2, y2, 1, h2);
            Rectangle r_otro_der = new Rectangle(x2+w2-1, y2, 1, h2);
            if (r_bola.IntersectsWith(r_otro_abajo) && !azarArriba)
            {
                velocidad_y *= -1;
            }
            else if (r_bola.IntersectsWith(r_otro_arriba))
            {
                velocidad_y *= -1;
                if (azarArriba)
                {
                    float normal = (float)(x - x2 + w) / (float)(w2 + w);
                    float angulo = (float)(3*Math.PI / 4) - normal* (float)(Math.PI / 2);
                    velocidad_x = 10 * (float)Math.Cos(angulo);
                    velocidad_y = 10 * -(float)Math.Sin(angulo);
                }
            }
            else if (r_bola.IntersectsWith(r_otro_izq) || r_bola.IntersectsWith(r_otro_der))
            {
                velocidad_x *= -1;
            }
            return (x < x2 + w2 &&
               x + w > x2 &&
               y < y2 + h2 &&
               h + y > y2);
        }
    }
}
