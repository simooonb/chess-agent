using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace ChessAgent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try {
                var agent = new Agent(PieceColor.White);
                var stop = false;
                var stopwatch = new Stopwatch();
                var tabVal = new int[64];
                string[] tabCoord = { "a8","b8","c8","d8","e8","f8","g8","h8",
                                "a7","b7","c7","d7","e7","f7","g7","h7",
                                "a6","b6","c6","d6","e6","f6","g6","h6",
                                "a5","b5","c5","d5","e5","f5","g5","h5",
                                "a4","b4","c4","d4","e4","f4","g4","h4",
                                "a3","b3","c3","d3","e3","f3","g3","h3",
                                "a2","b2","c2","d2","e2","f2","g2","h2",
                                "a1","b1","c1","d1","e1","f1","g1","h1" };
                string value;
                
                while (!stop)
                {
                    using (var mmf = MemoryMappedFile.OpenExisting("plateau"))
                    {
                        using(var mmf2 = MemoryMappedFile.OpenExisting("repAI1"))
                        {
                            var mutexStartAi1 = Mutex.OpenExisting("mutexStartAI1");
                            var mutexAi1 = Mutex.OpenExisting("mutexAI1");
                            mutexAi1.WaitOne();
                            
                            mutexStartAi1.WaitOne();

                            using (var accessor = mmf.CreateViewAccessor())
                            {
                                var size = accessor.ReadUInt16(0);
                                var buffer = new byte[size];
                                accessor.ReadArray(0 + 2, buffer, 0, buffer.Length);

                                value = Encoding.ASCII.GetString(buffer);
                                if (value == "stop") stop = true;
                                else
                                {
                                    var substrings = value.Split(',');
                                    for (var i = 0; i < substrings.Length; i++)
                                    {
                                        tabVal[i] = Convert.ToInt32(substrings[i]);
                                    }
                                }
                            }
                            if (!stop)
                            {
                                /******************************************************************************************************/
                                /***************************************** ECRIRE LE CODE DE L'IA *************************************/
                                /******************************************************************************************************/

                                stopwatch.Start();
                                    
                                var myPieces = new Dictionary<string, int>();
                                for (var i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] > 0) myPieces.Add(tabCoord[i], tabVal[i]);
                                }

                                var opponentPieces = new Dictionary<string, int>();
                                for (var i = 0; i < tabVal.Length; i++)
                                {
                                    if (tabVal[i] < 0) opponentPieces.Add(tabCoord[i], tabVal[i]);
                                }
                                
                                agent.ObserveEnvironmentAndUpdateState(myPieces, opponentPieces);
                                var coord = agent.ChooseMove();
                                
                                stopwatch.Stop();
                                
                                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                                Console.WriteLine(coord[0] + " " + coord[1]);
                                stopwatch.Reset();

                                // ex:
                                //coord[0] = "b2";
                                //coord[1] = "b3";
                                
                                /********************************************************************************************************/
                                /********************************************************************************************************/
                                /********************************************************************************************************/

                                using (var accessor = mmf2.CreateViewAccessor())
                                {
                                    value = coord[0];
                                    for (int i = 1; i < coord.Length; i++)
                                    {
                                        value += "," + coord[i];
                                    }
                                    var buffer = Encoding.ASCII.GetBytes(value);
                                    accessor.Write(0, (ushort)buffer.Length);
                                    accessor.WriteArray(0 + 2, buffer, 0, buffer.Length);
                                }
                            }
                            mutexAi1.ReleaseMutex();
                            mutexStartAi1.ReleaseMutex();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first.");
                Console.ReadLine();
            }
        }
    }
}