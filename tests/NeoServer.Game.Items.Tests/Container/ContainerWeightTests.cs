﻿using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Items.Tests.Container;

public class ContainerWeightTests
{
    [Fact]
    public void Container_weight_increases_when_items_are_added_to_it()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack(weight: 18);

        var weapon = ItemTestData.CreateWeaponItem(2, weight: 40);
        var fiveArrows = ItemTestData.CreateCumulativeItem(3, 5, weight: 0.70f);
        var twoArrows = ItemTestData.CreateCumulativeItem(3, 2, weight: 0.70f);

        //assert
        backpack.Weight.Should().Be(18);

        //act
        backpack.AddItem(weapon);

        //assert
        backpack.Weight.Should().Be(58);

        //act
        backpack.AddItem(fiveArrows);

        //assert
        backpack.Weight.Should().Be(61.5f);

        //act
        backpack.AddItem(twoArrows);

        //assert
        backpack.Weight.Should().Be(62.9f);
    }

    [Fact]
    public void Container_weight_decreases_when_items_are_removed_from_it()
    {
        //arrange

        var weapon = ItemTestData.CreateWeaponItem(2, weight: 40);
        var twoArrows = ItemTestData.CreateCumulativeItem(3, 2, weight: 0.70f);
        var twoMeat = ItemTestData.CreateFood(4, 2);

        var backpack = ItemTestData.CreateContainer(weight: 8, children: new List<IItem>
        {
            twoArrows, weapon, twoMeat
        });

        //assert
        backpack.Weight.Should().Be(75.40f);

        //act
        backpack.RemoveItem(weapon, 1);

        //assert
        backpack.Weight.Should().Be(35.4f);

        //act
        backpack.RemoveItem(twoArrows, 1);

        //assert
        backpack.Weight.Should().Be(34.7f);

        //act
        backpack.RemoveItem(twoArrows, 1);

        //assert
        backpack.Weight.Should().Be(34f);

        //act
        twoMeat.Reduce();

        //assert
        backpack.Weight.Should().Be(21f);

        //act
        twoMeat.Reduce();

        //assert
        backpack.Weight.Should().Be(8f);
    }
}