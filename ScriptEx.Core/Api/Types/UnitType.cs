using System;
using HotChocolate.Language;
using HotChocolate.Types;

namespace ScriptEx.Core.Api.Types
{
    public readonly struct Unit
    {
        public static Unit Default => default;

        public override string ToString() => "unit";
    }

    internal class UnitType : ScalarType<Unit, NullValueNode>
    {
        public UnitType() : base(nameof(Unit)) { }

        public override IValueNode ParseResult(object? resultValue) => throw new NotImplementedException();

        protected override Unit ParseLiteral(NullValueNode valueSyntax) => throw new NotImplementedException();

        protected override NullValueNode ParseValue(Unit runtimeValue) => throw new NotImplementedException();
    }
}
