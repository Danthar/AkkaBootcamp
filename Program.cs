using System;
using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActionSystem");

            Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
            ActorRef consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

            Props validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
            ActorRef validationActor = MyActorSystem.ActorOf(validationActorProps, "validationActor");
            
            Props consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
            ActorRef consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

            // tell console reader to begin
            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);
            
            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.AwaitTermination();
        }
    }
    #endregion
}
