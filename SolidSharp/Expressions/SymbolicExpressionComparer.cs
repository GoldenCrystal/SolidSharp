﻿using SolidSharp.Expressions.Extensions;
using System;
using System.Collections.Generic;

namespace SolidSharp.Expressions
{
	internal sealed class SymbolicExpressionComparer : IComparer<SymbolicExpression>
	{
		public const byte Number = 0;
		public const byte Constant = 1;
		public const byte Variable = 2;
		public const byte Unary = 3;
		public const byte Addition = 4;
		public const byte Subtraction = 5;
		public const byte Multiplication = 6;
		public const byte Division = 7;
		public const byte Root = 8;
		public const byte Power = 9;

		public static readonly SymbolicExpressionComparer Default = new SymbolicExpressionComparer();

		private SymbolicExpressionComparer() { }

		public int Compare(SymbolicExpression x, SymbolicExpression y)
		{
			var (ux, uy) = (ExpressionSimplifier.UnwrapFactors(x), ExpressionSimplifier.UnwrapFactors(y));

			int result = Comparer<byte>.Default.Compare(ux.Expression.GetSortOrder(), uy.Expression.GetSortOrder());

			if (result == 0)
			{
				result = CompareUnwrapped(ux.Expression, uy.Expression);
			}

			if (result == 0)
			{
				result = Comparer<long>.Default.Compare(ux.Factor, uy.Factor);
			}

			return result;
		}

		private static int CompareUnwrapped(SymbolicExpression x, SymbolicExpression y)
		{
			if (x.Kind == y.Kind)
			{
				switch (x.Kind)
				{
					case ExpressionKind.Number:
						return Comparer<long>.Default.Compare(x.GetValue(), y.GetValue());
					case ExpressionKind.Constant:
						return Comparer<byte>.Default.Compare(((ConstantExpression)x).ConstantSortOrder, ((ConstantExpression)y).ConstantSortOrder);
					case ExpressionKind.Variable:
						return StringComparer.Ordinal.Compare(((VariableExpression)x).Name, ((VariableExpression)y).Name);
				}
			}

			return 0;
		}
	}
}
