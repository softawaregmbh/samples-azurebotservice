using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "Are you sure you want to reset the count?",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
        }
        else
        {
            await context.PostAsync("Hmm..");
            
            var typingActivity = context.MakeMessage();
            typingActivity.Type = "typing";
            
            await context.PostAsync(typingActivity);
            
            await Task.Delay(3000);
            
            var responseMessage = context.MakeMessage();

            var card = new HeroCard()
            {
                Title = "Cognitive Services",
                Subtitle = "Roman Schacherl",
                Text = $"In Kino findet ein Vortrag über Cognitive Services statt.",
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = "https://api-summit.de/wp-content/uploads/2017/03/API_Summit-3914.jpg",
                        Tap = new CardAction(Microsoft.Bot.Connector.ActionTypes.OpenUrl, "Open", null, "https://www.api-summit.de")
                    }
                }
            };

            responseMessage.Attachments.Add(card.ToAttachment());

            await context.PostAsync(responseMessage);
            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            this.count = 1;
            await context.PostAsync("Reset count.");
        }
        else
        {
            await context.PostAsync("Did not reset count.");
        }
        context.Wait(MessageReceivedAsync);
    }
}