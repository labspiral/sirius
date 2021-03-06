﻿/*
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 레이저및 스캐너의 가공 파라메터를 일컬어 통상 "펜(Pen)" 파라메터라 하며, 이 펜 객체(Entity)를 사용해 다양한 가공 조건 (속도및 지연값등)을 설정한다.
 * Author : hong chan, choi / labspiral@gmail.com(http://spirallab.co.kr)
 * 
 */


using System;
using System.Diagnostics;
using System.IO;

namespace SpiralLab.Sirius
{
    class Program
    {
        static void Main(string[] args)
        {
            SpiralLab.Core.Initialize();

            #region initialize RTC 
            //var rtc = new RtcVirtual(0); //create Rtc for dummy
            var rtc = new Rtc5(0); //create Rtc5 controller
            //var rtc = new Rtc6(0); //create Rtc6 controller
            //var rtc = new Rtc6Ethernet(0, "192.168.0.100", "255.255.255.0"); //실험적인 상태 (Scanlab Rtc6 Ethernet 제어기)
            //var rtc = new Rtc6SyncAxis(0, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration", "syncAXISConfig.xml")); //실험적인 상태 (Scanlab XLSCAN 솔류션)

            float fov = 60.0f;    // scanner field of view : 60mm                                
            float kfactor = (float)Math.Pow(2, 20) / fov; // k factor (bits/mm) = 2^20 / fov
            var correctionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction", "cor_1to1.ct5");
            rtc.Initialize(kfactor, LaserMode.Yag1, correctionFile);    //default correction file
            rtc.CtlFrequency(50 * 1000, 2); //laser frequency : 50KHz, pulse width : 2usec
            rtc.CtlSpeed(100, 100); // default jump and mark speed : 100mm/s
            rtc.CtlDelay(10, 100, 200, 200, 0); //scanner and laser delays
            #endregion

            #region initialize Laser (virtual)
            ILaser laser = new LaserVirtual(0, "virtual", 20);
            #endregion

            #region create entities
            // 문서 생성
            var doc = new DocumentDefault("Unnamed");
            // 레이어 생성및 문서에 추가
            var layer = new Layer("default");
            // 펜 개체(Entity) 생성및 레이어에 추가            
            var pen = new PenDefault()
            {
                Frequency = 100 * 1000, //주파수 Hz
                PulseWidth = 2, //펄스폭 usec
                LaserOnDelay = 0, // 레이저 시작 지연 usec
                LaserOffDelay = 0, // 레이저 끝 지연 usec
                ScannerJumpDelay = 100, // 스캐너 점프 지연 usec
                ScannerMarkDelay = 200, // 스캐너 마크 지연 usec
                ScannerPolygonDelay = 0, // 스캐너 폴리곤 지연 usec
                JumpSpeed = 500, // 스캐너 점프 속도 mm/s
                MarkSpeed = 500, // 스캐너 마크 속도 mm/s
            };
            layer.Add(pen);
            // 선 개체 레이어에 추가
            layer.Add(new Line(0, 0, 10, 20));
            // 원 개체 레이어에 추가
            layer.Add(new Circle(0, 0, 10));
            // 나선 개체 레이어에 추가
            layer.Add(new Spiral(-20.0f, 0.0f, 0.5f, 2.0f, 5, true));            
            //레이어의 모든 개채들 내부 데이타 계산및 갱신
            layer.Regen();
            // 문서에 레이어 추가
            doc.Layers.Add(layer);

            // 문서를 지정된 파일에 저장
            DocumentSerializer.Save(doc, "test.sirius");
            #endregion

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine($"{Environment.NewLine}");
                Console.WriteLine("Testcase for spirallab.sirius. powered by labspiral@gmail.com (http://spirallab.co.kr)");
                Console.WriteLine($"{Environment.NewLine}");
                Console.WriteLine("'D' : draw entities by pen");
                Console.WriteLine("'Q' : quit");
                Console.WriteLine($"{Environment.NewLine}");
                Console.Write("select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D:
                        Console.WriteLine("WARNING !!! LASER IS BUSY ...");
                        var timer = Stopwatch.StartNew();
                        DrawForFieldCorrection(laser, rtc, doc);
                        Console.WriteLine($"processing time = {timer.ElapsedMilliseconds / 1000.0:F3}s");
                        break;
                }

            } while (true);

            rtc.Dispose();
        }
        /// <summary>
        /// 레이어 안에 있는 모든 객체들을 마킹하기 (3x3 의 나선 객체가 마킹됨)
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="doc"></param>
        private static bool DrawForFieldCorrection(ILaser laser, IRtc rtc, IDocument doc)
        {
            bool success = true;
            var markerArg = new MarkerArgDefault()
            {
                Document = doc,
                Rtc = rtc,
                Laser = laser,
            };
            success &= rtc.ListBegin(laser);
            // 레이어 순회
            foreach (var layer in doc.Layers)
            {
                //레이어 가공
                success &= layer.Mark(markerArg);
                // or
                // 직접 하나씩 처리방법. 레이어 내의 개체들을 순회
                //foreach (var entity in layer)
                //{
                //    var markerable = entity as IMarkerable;
                //    // 해당 개체가 레이저 가공이 가능한지 여부를 판별
                //    if (null != markerable)
                //        success &= markerable.Mark(markerArg);    // 레이저 가공 실시
                //    if (!success)
                //        break;
                //}
                if (!success)
                    break;
            }
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }        
    }
}
