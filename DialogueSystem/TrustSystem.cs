using System;
using System.Collections.Generic;

namespace MysteriousCallouts.DialogueSystem;

public class TrustSystem
{
    public Conversation conversation { get; set; }
    public Dictionary<QuestionEffect, int> effectValues { get; set; }
    public int negativeThreshold { get; set; }
    public int positiveThreshold { get; set; }
    public int trustLevel { get; set; }

    public TrustSystem(Conversation conversation, int[] effectArray, int positiveThreshold, int negativeThreshold, int trustLevel)
    {
        this.conversation = conversation;
        this.trustLevel = trustLevel;
        this.positiveThreshold = positiveThreshold;
        this.negativeThreshold = negativeThreshold;
        effectValues = new Dictionary<QuestionEffect, int>()
        {
            { QuestionEffect.NEGATIVE, effectArray[0] },
            { QuestionEffect.NEUTRAL, effectArray[1] },
            { QuestionEffect.POSITIVE, effectArray[2] },
        };
    }
    
    private void CalculateFinalTrustLevel()
    {
        int pos = conversation.numberOfPositive;
        int neg = conversation.numberOfNegative;
        int neutral = conversation.numberOfNeutral;

        trustLevel += (pos * effectValues[QuestionEffect.POSITIVE]);
        trustLevel += (neutral * effectValues[QuestionEffect.NEUTRAL]);
        trustLevel += (neg * effectValues[QuestionEffect.NEGATIVE]);
    }
    
    public QuestionEffect CalculateFinalEffect()
    {
        CalculateFinalTrustLevel();
        if (trustLevel <= negativeThreshold)
        {
            return QuestionEffect.NEGATIVE;
        }
        else if (trustLevel < positiveThreshold && trustLevel > negativeThreshold)
        {
            return QuestionEffect.NEUTRAL;
        }
        else
        {
            return QuestionEffect.POSITIVE;
        }
    }

    public void CalculateFinalEffect(Action[] arrayOfEffects)
    {
        CalculateFinalTrustLevel();
        QuestionEffect calculatedEffect;
        if (trustLevel <= negativeThreshold)
        {
            calculatedEffect = QuestionEffect.NEGATIVE;
        }
        else if (trustLevel < positiveThreshold && trustLevel > negativeThreshold)
        {
            calculatedEffect = QuestionEffect.NEUTRAL;
        }
        else
        {
            calculatedEffect = QuestionEffect.POSITIVE;
        }
        
        switch (calculatedEffect)
        {
            case QuestionEffect.POSITIVE:
                arrayOfEffects[2]();
                break;
            case QuestionEffect.NEUTRAL:
                arrayOfEffects[1]();
                break;
            case QuestionEffect.NEGATIVE:
                arrayOfEffects[0]();
                break;
        }
    }
}