using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Tetromino
    {
        // Все возможные формы фигур Тетриса
        private static readonly int[][,] Shapes = new int[][,]
        {
            // I-фигура (палка)
            new int[,] {
                {1, 1, 1, 1}
            },
            
            // O-фигура (квадрат)
            new int[,] {
                {1, 1},
                {1, 1}
            },
            
            // T-фигура
            new int[,] {
                {0, 1, 0},
                {1, 1, 1}
            },
            
            // S-фигура
            new int[,] {
                {0, 1, 1},
                {1, 1, 0}
            },
            
            // Z-фигура
            new int[,] {
                {1, 1, 0},
                {0, 1, 1}
            },
            
            // J-фигура
            new int[,] {
                {1, 0, 0},
                {1, 1, 1}
            },
            
            // L-фигура
            new int[,] {
                {0, 0, 1},
                {1, 1, 1}
            }
        };
        
        // Свойства фигуры
        public int[,] Shape { get; set; }  // Матрица фигуры
        public int X { get; set; }                 // Позиция X на поле
        public int Y { get; set; }                 // Позиция Y на поле
        public int Type { get; private set; }      // Тип фигуры (0-6)
        
        // Конструктор - создает случайную фигуру
        public Tetromino()
        {
            Random random = new Random();
            Type = random.Next(0, Shapes.Length);
            Shape = (int[,])Shapes[Type].Clone();
            X = 4;  // Начальная позиция по центру
            Y = 0;  // Вверху поля
        }
        
        // Создание конкретной фигуры (для тестов)
        public Tetromino(int type)
        {
            if (type < 0 || type >= Shapes.Length)
                throw new ArgumentException("Неверный тип фигуры");
                
            Type = type;
            Shape = (int[,])Shapes[type].Clone();
            X = 4;
            Y = 0;
        }

        public void SetShape(int[,] newShape)
        {
            Shape = newShape;
        }

        // Вращение фигуры по часовой стрелке
        public void Rotate()
        {
            int rows = Shape.GetLength(0);
            int cols = Shape.GetLength(1);
            
            // Создаем новую матрицу для повернутой фигуры
            int[,] rotated = new int[cols, rows];
            
            // Поворачиваем на 90 градусов
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotated[j, rows - 1 - i] = Shape[i, j];
                }
            }
            
            Shape = rotated;
        }
        
        // Вращение против часовой стрелки
        public void RotateCounterClockwise()
        {
            int rows = Shape.GetLength(0);
            int cols = Shape.GetLength(1);
            
            int[,] rotated = new int[cols, rows];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotated[cols - 1 - j, i] = Shape[i, j];
                }
            }
            
            Shape = rotated;
        }
        
        // Получение ширины фигуры
        public int Width => Shape.GetLength(1);
        
        // Получение высоты фигуры
        public int Height => Shape.GetLength(0);
    }
}
