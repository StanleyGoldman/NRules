﻿using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.Samples.MissManners.Domain;

namespace NRules.Samples.MissManners.Rules
{
    [Name("FindSeating")]
    public class FindSeating : Rule
    {
        public override void Define()
        {
            Context context = null;
            Seating seating = null;
            Guest guest1 = null;
            Guest guest2 = null;
            Count count = null;

            When()
                .Match<Context>(() => context, c => c.State == ContextState.AssignSeats)
                .Match<Seating>(() => seating, s => s.PathDone)
                .Match<Guest>(() => guest1, g1 => g1.Name == seating.RightGuestName)
                .Match<Guest>(() => guest2, g2 => g2.Sex != guest1.Sex, g2 => g2.Hobby == guest1.Hobby)
                .Match<Count>(() => count)
                .Not<Path>(p => p.Id == seating.Id, p => p.GuestName == guest2.Name)
                .Not<Chosen>(c => c.Id == seating.Id, c => c.GuestName == guest2.Name, c => c.Hobby == guest1.Hobby);

            Then()
                .Do(ctx => AssignSeat(ctx, context, seating, guest1, guest2, count));
        }

        private void AssignSeat(IContext ctx, Context context, Seating seating, Guest guest1, Guest guest2, Count count)
        {
            int rightSeat = seating.RightSeatId;
            int seatId = seating.Id;
            int cnt = count.Value;

            var newSeating = new Seating(cnt, seatId, false, rightSeat, seating.RightGuestName, rightSeat + 1, guest2.Name);
            ctx.Insert(newSeating);

            var path = new Path(cnt, rightSeat + 1, guest2.Name);
            ctx.Insert(path);

            var chosen = new Chosen(seatId, guest2.Name, guest1.Hobby);
            ctx.Insert(chosen);

            count.Increment();
            ctx.Update(count);

            context.SetState(ContextState.MakePath);
            ctx.Update(context);
        }
    }
}
