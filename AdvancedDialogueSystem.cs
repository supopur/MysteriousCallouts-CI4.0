using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace MysteriousCallouts
{
    public class AdvancedDialogueSystem
    {
        public class QuestionAndAnswer
        {
            public enum QuestionEffect
            {
                POSITIVE ,
                NEUTRAL,
                NEGATIVE
            }
            public string question { get; set; }
            public QuestionEffect effect { get; set; }
            
            public string answer { get; set; }

            public QuestionAndAnswer(string question, QuestionEffect effect, string answer)
            {
                this.question = question;
                this.effect = effect;
                this.answer = answer;
            }
        }

        public class QuestionPool
        {
            public List<QuestionAndAnswer> pool { get; set; }
            public QuestionPool(List<QuestionAndAnswer> pool)
            {
                this.pool = pool;
            }

            public string DisplayQuestions()
            {
                string displaystr = "";
                for(int i = 0; i< pool.Count; i++)
                {
                    displaystr += $"[{i}]: {pool[i].question}\n";
                }
                return displaystr;
            }

            public string GetAnswer(int index)
            {
                return pool[index].answer;
            }
            
            public QuestionAndAnswer.QuestionEffect GetEffect(int index)
            {
                return pool[index].effect;
            }

            public bool IsValidIndex(int index) => index < pool.Count;
        }

        public class Conversation
        {
            public int startingTrustLevel { get; private set; }
            
            public int numberOfPositive { get; private set; }
            public int numberOfNegative { get; private set; }
            public int numberOfNeutral { get; private set; }

            public Dictionary<QuestionAndAnswer.QuestionEffect, int> effectValues { get; set; }
            public List<QuestionPool> dialouge { get; set; }
            static Keys[] validKeys = new[]
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };

            static Keys[] numpadKeys = new[]
            {
                Keys.NumPad0,Keys.NumPad1,Keys.NumPad2,Keys.NumPad3,Keys.NumPad4,Keys.NumPad5,Keys.NumPad6,Keys.NumPad7,Keys.NumPad8,Keys.NumPad9
            };

            public Conversation(int startingTrustLevel, List<QuestionPool> dialouge, int[] effectArray, bool useNumpadKeys)
            {
                this.startingTrustLevel = startingTrustLevel;
                this.dialouge = dialouge;
                effectValues = new Dictionary<QuestionAndAnswer.QuestionEffect, int>()
                {
                    { QuestionAndAnswer.QuestionEffect.NEGATIVE, effectArray[0] },
                    { QuestionAndAnswer.QuestionEffect.NEUTRAL, effectArray[1] },
                    { QuestionAndAnswer.QuestionEffect.POSITIVE, effectArray[2] },
                };
                if (useNumpadKeys)
                {
                    validKeys = numpadKeys;
                }
            }

            public void AffectTrustLevel(QuestionAndAnswer.QuestionEffect effect)
            {
                switch (effect)
                {
                    case QuestionAndAnswer.QuestionEffect.POSITIVE:
                        numberOfPositive++;
                        break;
                    case QuestionAndAnswer.QuestionEffect.NEUTRAL:
                        numberOfNeutral++;
                        break;
                    case QuestionAndAnswer.QuestionEffect.NEGATIVE:
                        numberOfNegative++;
                        break;
                }
                startingTrustLevel -= effectValues[effect];
            }
            
            public int WaitForValidKeyPress(QuestionPool q)
            {
                bool isValidKeyPressed = false;
                int indexPressed = 0;
                while (!isValidKeyPressed)
                {
                    GameFiber.Yield();
                    for (int i = 0; i < validKeys.Length; i++)
                    {
                        Keys key = validKeys[i];
                        if (Game.IsKeyDown(key) && q.IsValidIndex(i))
                        {
                            isValidKeyPressed = true;
                            indexPressed = i;
                        }
                    }
                }
                return indexPressed;
            }
            
            public void Run()
            {
                GameFiber.StartNew(delegate
                {
                    foreach (QuestionPool q in dialouge)
                    {
                        Game.DisplayHelp(q.DisplayQuestions(),10000);
                        int indexPressed = WaitForValidKeyPress(q);
                        Game.HideHelp();
                        Game.DisplaySubtitle(q.GetAnswer(indexPressed));
                        AffectTrustLevel(q.GetEffect(indexPressed));
                    }
                });
            }

            public void AddQuestionsToMenu(UIMenu menu, QuestionPool q)
            {
                foreach (QuestionAndAnswer qanda in q.pool)
                {
                    menu.AddItem(new UIMenuItem(qanda.question));
                }
            }

            public void OnItemSelect(int index, QuestionPool q)
            {
                Game.DisplaySubtitle(q.GetAnswer(index));
                AffectTrustLevel(q.GetEffect(index));
            }
        }

    }
}