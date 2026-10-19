using System;
using NUnit.Framework;

namespace Air.GameCore.Tests
{
    enum DemoEvent : uint
    {
        Pet = 1000,
        Bag = 1001
    }

    public sealed class EventTests
    {
        [SetUp]
        public void SetUp() => EventManager.Instance.Reset();

        [TearDown]
        public void TearDown() => EventManager.Instance.Reset();

        [Test]
        public void AttachAgainReplacesThePreviousCallback()
        {
            var node = new ViTupleCallback<ulong>();
            var seen = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, node, (id, uid) => seen++);
            EventManager.Instance.Attach(DemoEvent.Pet, node, (id, uid) => seen += 10);
            EventManager.Instance.Invoke(DemoEvent.Pet, 4UL);
            Assert.AreEqual(10, seen);
            Assert.IsTrue(node.Active);
            node.End();
            node.End();
            Assert.IsFalse(node.Active);
            EventManager.Instance.Invoke(DemoEvent.Pet, 4UL);
            Assert.AreEqual(10, seen);
        }

        [Test]
        public void EndingALaterNodeDuringInvokeSkipsIt()
        {
            var first = new ViTupleCallback();
            var second = new ViTupleCallback();
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, first, id => second.End());
            EventManager.Instance.Attach(DemoEvent.Pet, second, id => hits++);
            EventManager.Instance.Invoke(DemoEvent.Pet);
            Assert.AreEqual(0, hits);
            Assert.IsFalse(second.Active);
        }

        [Test]
        public void NodeAttachedDuringInvokeWaitsForTheNextBroadcast()
        {
            var first = new ViTupleCallback();
            var extra = new ViTupleCallback();
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, first, id =>
                EventManager.Instance.Attach(DemoEvent.Pet, extra, _ => hits++));
            EventManager.Instance.Invoke(DemoEvent.Pet);
            Assert.AreEqual(0, hits);
            EventManager.Instance.Invoke(DemoEvent.Pet);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void AsyncNodeKeepsOnlyTheLastArgumentsInOneFrame()
        {
            var node = new ViAsynTupleCallback<ulong>();
            ulong seen = 0;
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, node, (id, uid) =>
            {
                hits++;
                seen = uid;
            });
            EventManager.Instance.Invoke(DemoEvent.Pet, 1UL);
            EventManager.Instance.Invoke(DemoEvent.Pet, 9UL);
            Assert.AreEqual(0, hits);
            EventManager.Instance.Flush();
            Assert.AreEqual(1, hits);
            Assert.AreEqual(9UL, seen);
        }

        [Test]
        public void AsyncEnqueueDuringFlushWaitsForTheNextFlush()
        {
            var node = new ViAsynTupleCallback();
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Bag, node, id =>
            {
                hits++;
                if (hits == 1)
                    EventManager.Instance.Invoke(DemoEvent.Bag);
            });
            EventManager.Instance.Invoke(DemoEvent.Bag);
            EventManager.Instance.Flush();
            Assert.AreEqual(1, hits);
            EventManager.Instance.Flush();
            Assert.AreEqual(2, hits);
        }

        [Test]
        public void EndingAMiddleListenerStillReachesTheTail()
        {
            var first = new ViTupleCallback();
            var middle = new ViTupleCallback();
            var last = new ViTupleCallback();
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, first, id => middle.End());
            EventManager.Instance.Attach(DemoEvent.Pet, middle, id => hits += 10);
            EventManager.Instance.Attach(DemoEvent.Pet, last, id => hits++);
            EventManager.Instance.Invoke(DemoEvent.Pet);
            Assert.AreEqual(1, hits);
            Assert.IsFalse(middle.Active);
            Assert.IsTrue(last.Active);
        }

        [Test]
        public void ReattachDuringInvokeDoesNotWalkTheOtherChain()
        {
            var first = new ViTupleCallback();
            var moved = new ViTupleCallback();
            var hits = 0;
            EventManager.Instance.Attach(DemoEvent.Pet, first, id =>
                EventManager.Instance.Attach(DemoEvent.Bag, moved, _ => hits++));
            EventManager.Instance.Attach(DemoEvent.Pet, moved, id => hits += 10);
            EventManager.Instance.Invoke(DemoEvent.Pet);
            Assert.AreEqual(0, hits);
            EventManager.Instance.Invoke(DemoEvent.Bag);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void EndingAQueuedAsyncNodeDoesNotDropTheRest()
        {
            var first = new ViAsynTupleCallback();
            var middle = new ViAsynTupleCallback();
            var last = new ViAsynTupleCallback();
            var hits = "";
            EventManager.Instance.Attach(DemoEvent.Pet, first, id =>
            {
                hits += "A";
                middle.End();
            });
            EventManager.Instance.Attach(DemoEvent.Pet, middle, id => hits += "B");
            EventManager.Instance.Attach(DemoEvent.Pet, last, id => hits += "C");
            EventManager.Instance.Invoke(DemoEvent.Pet);
            EventManager.Instance.Flush();
            Assert.AreEqual("AC", hits);
            EventManager.Instance.Flush();
            Assert.AreEqual("AC", hits);
            Assert.IsFalse(middle.Active);
        }
    }

    public sealed class TimerTests
    {
        [Test]
        public void ShortDelayFiresOnTheTickItComesDue()
        {
            var timer = NewTimer();
            var node = new ViTimeNode1();
            var hits = 0;
            timer.SetTime(node, 0.5f, _ => hits++);
            timer.Update(0.49f);
            Assert.AreEqual(0, hits);
            Assert.IsTrue(node.IsAttach());
            timer.Update(0.02f);
            Assert.AreEqual(1, hits);
            Assert.IsFalse(node.IsAttach());
        }

        [Test]
        public void DelayPastThreeSecondsUsesTheSecondWheel()
        {
            var timer = NewTimer();
            var node = new ViTimeNode1();
            var hits = 0;
            timer.SetTime(node, 4f, _ => hits++);
            timer.Update(3f);
            Assert.AreEqual(0, hits);
            timer.Update(1f);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void DelayPastFifteenMinutesUsesTheReserveList()
        {
            var timer = NewTimer();
            var node = new ViTimeNode1();
            var hits = 0;
            timer.SetTime(node, 16f * 60f, _ => hits++);
            timer.Update(15f * 60f);
            Assert.AreEqual(0, hits);
            timer.Update(60f);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void AThrowingCallbackDoesNotCancelTheRestOfTheSlot()
        {
            var timer = NewTimer();
            var errors = 0;
            var previous = ViTimerLog.Exception;
            ViTimerLog.Exception = _ => errors++;
            try
            {
                var hits = 0;
                timer.SetTime(new ViTimeNode1(), 0.1f, _ => throw new InvalidOperationException("fail"));
                timer.SetTime(new ViTimeNode1(), 0.1f, _ => hits++);
                timer.Update(0.1f);
                Assert.AreEqual(1, hits);
                Assert.AreEqual(1, errors);
            }
            finally
            {
                ViTimerLog.Exception = previous;
            }
        }

        [Test]
        public void CallbackCanScheduleTheSameNodeAgain()
        {
            var timer = NewTimer();
            var node = new ViTimeNode1();
            var hits = 0;
            void Tick(ViTimeNodeInterface _)
            {
                hits++;
                if (hits < 3)
                    timer.SetTime(node, 0.1f, Tick);
            }

            timer.SetTime(node, 0.1f, Tick);
            timer.Update(0.1f);
            timer.Update(0.1f);
            timer.Update(0.1f);
            Assert.AreEqual(3, hits);
        }

        [Test]
        public void ResetTimeAndModifyOnlyAffectAnAttachedNode()
        {
            var timer = NewTimer();
            var node = new ViTimeNode1();
            var hits = 0;
            timer.ResetTime(node, 0.1f);
            timer.Modify(node, -5);
            timer.SetTime(node, 0.5f, _ => hits++);
            timer.Update(0.4f);
            timer.ResetTime(node, 0.5f);
            timer.Update(0.4f);
            Assert.AreEqual(0, hits);
            timer.Modify(node, 10);
            timer.Update(0.1f);
            Assert.AreEqual(0, hits);
            timer.Update(0.1f);
            Assert.AreEqual(1, hits);
            node.Detach();
            node.Detach();
            Assert.IsFalse(node.IsAttach());
        }

        [Test]
        public void SetTimeMovesANodeOffItsPreviousTimer()
        {
            var first = NewTimer();
            var second = NewTimer();
            var node = new ViTimeNode1();
            var fromFirst = 0;
            var fromSecond = 0;
            first.SetTime(node, 0.2f, _ => fromFirst++);
            second.SetTime(node, 0.2f, _ => fromSecond++);
            first.Update(0.2f);
            second.Update(0.2f);
            Assert.AreEqual(0, fromFirst);
            Assert.AreEqual(1, fromSecond);
        }

        [Test]
        public void RealTimeBudgetCapsEachStepAtHalfASecond()
        {
            var budget = new RealTimeBudget();
            Assert.AreEqual(0.5f, budget.Consume(1.2f));
            Assert.AreEqual(0.5f, budget.Consume(0f));
            Assert.AreEqual(0.2f, budget.Consume(0f), 0.0001f);
        }

        [Test]
        public void TimePoolCancelsByKeyAndOnEnd()
        {
            var timer = NewTimer();
            var pool = new ViTimePool(timer);
            var hits = 0;
            pool.Wait(0.5f, () => hits++);
            pool.Wait("catch", 0.2f, (ulong uid) => hits += (int)uid, 3UL);
            Assert.IsFalse(pool.IsComplete("catch"));
            Assert.Throws<InvalidOperationException>(() => pool.Wait("catch", 0.2f, () => hits++));
            pool.CancelWait("catch");
            Assert.IsTrue(pool.IsComplete("catch"));
            timer.Update(1f);
            Assert.AreEqual(1, hits);

            pool.Wait(0.2f, () => hits++);
            pool.End();
            timer.Update(1f);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void ReturningAPooledNodeDetachesIt()
        {
            var timer = NewTimer();
            var node = ViTimeNodePool.Get();
            var hits = 0;
            timer.SetTime(node, 0.2f, _ => hits++);
            ViTimeNodePool.Return(node);
            timer.Update(0.2f);
            Assert.AreEqual(0, hits);
            Assert.IsFalse(node.IsAttach());
        }

        [Test]
        public void UpdateFromCallbackDoesNotDropTheRestOfTheSlot()
        {
            var timer = NewTimer();
            var hits = 0;
            timer.SetTime(new ViTimeNode1(), 0.1f, _ => hits++);
            timer.SetTime(new ViTimeNode1(), 0.1f, _ => timer.Update(0.1f));
            timer.Update(0.1f);
            Assert.AreEqual(1, hits);
            timer.Update(0.1f);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void FailedRestartKeepsExistingTimers()
        {
            var timer = new ViVisualCustomTimerInstance();
            timer.Start(0, 0, 1f, 300, 300);
            var hits = 0;
            timer.SetTime(new ViTimeNode1(), 0.1f, _ => hits++);
            Assert.Throws<ArgumentOutOfRangeException>(() => timer.Start(0, 0, 0f, 0, 300));
            timer.Update(0.1f);
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void ZeroScalePausesACustomTimer()
        {
            var timer = new ViVisualCustomTimerInstance();
            timer.Start(0, 0, 0f, 300, 300);
            var hits = 0;
            timer.SetTime(new ViTimeNode1(), 0.1f, _ => hits++);
            timer.Update(1f);
            Assert.AreEqual(0, hits);

            timer.Start(0, 0, 1f, 300, 300);
            var node = new ViTimeNode1();
            timer.SetTime(node, 0.2f, _ => hits++);
            timer.End();
            timer.Update(1f);
            Assert.AreEqual(0, hits);
            Assert.IsFalse(node.IsAttach());
        }

        static ViVisualCustomTimerInstance NewTimer()
        {
            var timer = new ViVisualCustomTimerInstance();
            timer.Start(0, 0, 1f, 300, 300);
            return timer;
        }
    }
}
