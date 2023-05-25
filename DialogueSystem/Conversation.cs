using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace MysteriousCallouts.DialogueSystem;

public class Conversation
{
    public int numberOfPositive { get; private set; }
    public int numberOfNegative { get; private set; }
    public int numberOfNeutral { get; private set; }
    public List<QuestionPool> dialouge { get; set; }
    
    private static Keys[] validKeys = new[]
    {
        Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
    };
    
    private static Keys[] numpadKeys = new[]
    {
        Keys.NumPad0,Keys.NumPad1,Keys.NumPad2,Keys.NumPad3,Keys.NumPad4,Keys.NumPad5,Keys.NumPad6,Keys.NumPad7,Keys.NumPad8,Keys.NumPad9
    };
    
    public Conversation(List<QuestionPool> dialouge,bool useNumpadKeys)
    {
        this.dialouge = dialouge;
        numberOfNegative = 0;
        numberOfNeutral = 0;
        numberOfPositive = 0;
        if (useNumpadKeys)
        {
            validKeys = numpadKeys;
        }
    }
    
    private void UpdateNumbers(QuestionEffect effect)
    {
        switch (effect)
        {
            case QuestionEffect.POSITIVE:
                numberOfPositive++;
                break;
            case QuestionEffect.NEUTRAL:
                numberOfNeutral++;
                break;
            case QuestionEffect.NEGATIVE:
                numberOfNegative++;
                break;
        }
    }
    
    private int WaitForValidKeyPress(QuestionPool q)
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
                UpdateNumbers(q.GetEffect(indexPressed));
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
        UpdateNumbers(q.GetEffect(index));
    }
}