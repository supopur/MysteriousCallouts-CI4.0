using System.Collections.Generic;

namespace MysteriousCallouts.DialogueSystem;

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
            
    public QuestionEffect GetEffect(int index)
    {
        return pool[index].effect;
    }

    public bool IsValidIndex(int index) => index < pool.Count;
}