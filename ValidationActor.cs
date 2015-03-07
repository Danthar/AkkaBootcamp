using Akka.Actor;

namespace WinTail {
    public class ValidationActor : UntypedActor {
        private readonly ActorRef _consoleWriterActor;

        public ValidationActor(ActorRef consoleWriterActor) {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message) {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg)) {
                //signal that the user needs to supply an input
                _consoleWriterActor.Tell(new Messages.NullInputError("No input recieved"));
            }
            else {
                var valid = IsValid(msg);
                if (valid) {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else {
                    //signal that input was bad
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        /// <summary>
        /// Determines if the message received is valid.
        /// Currently, arbitrarily checks if number of chars in message received is even.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static bool IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }

    }
}