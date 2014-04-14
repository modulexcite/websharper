// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2013 IntelliFactory
//
// GNU Affero General Public License Usage
// WebSharper is free software: you can redistribute it and/or modify it under
// the terms of the GNU Affero General Public License, version 3, as published
// by the Free Software Foundation.
//
// WebSharper is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License
// for more details at <http://www.gnu.org/licenses/>.
//
// If you are unsure which license is appropriate for your use, please contact
// IntelliFactory at http://intellifactory.com/contact.
//
// $end{copyright}

namespace IntelliFactory.WebSharper.Sitelets

/// Represents a self-contained website parameterized by the type of actions.
/// A sitelet combines a router, which is used to match incoming requests to
/// actions and actions to URLs, and a controller, which is used to handle
/// the actions.
type Sitelet<'T when 'T : equality> =
    {
        Router : Router<'T>
        Controller : Controller<'T>
    }

    /// Combines two sitelets, with the leftmost taking precedence.
    static member ( <|> ) : Sitelet<'Action> * Sitelet<'Action> -> Sitelet<'Action>

/// Provides combinators over sitelets.
module Sitelet =

    /// Creates an empty sitelet.
    val Empty<'Action when 'Action : equality> : Sitelet<'Action>

    /// Represents filters for protecting sitelets.
    type Filter<'Action> =
        {
            VerifyUser : string -> bool
            LoginRedirect : 'Action -> 'Action
        }

    /// Constructs a protected sitelet given the filter specification.
    val Protect<'T when 'T : equality> :
        filter: Filter<'T> ->
        site: Sitelet<'T> ->
        Sitelet<'T>

    /// Constructs a singleton sitelet that contains exactly one action
    /// and serves a single content value at a given location.
    val Content<'T when 'T : equality> :
        location: string ->
        action: 'T ->
        cnt: Content<'T> ->
        Sitelet<'T>

    /// Maps over the sitelet action type. Requires a bijection.
    val Map<'T1,'T2 when 'T1 : equality and 'T2 : equality> :
        ('T1 -> 'T2) -> ('T2 -> 'T1) -> Sitelet<'T1> -> Sitelet<'T2>

    /// Shifts all sitelet locations by a given prefix.
    val Shift<'T when 'T : equality> :
        prefix: string -> sitelet: Sitelet<'T> -> Sitelet<'T>

    /// Combines several sitelets, leftmost taking precedence.
    /// Is equivalent to folding with the choice operator.
    val Sum<'T when 'T : equality> :
        sitelets: seq<Sitelet<'T>> -> Sitelet<'T>

    /// Serves the sum of the given sitelets under a given prefix.
    /// This function is convenient for folder-like structures.
    val Folder<'T when 'T : equality> :
        prefix: string -> sitelets: seq<Sitelet<'T>> -> Sitelet<'T>

    /// Boxes the sitelet action type to Object type.
    val Upcast<'T when 'T : equality> :
        sitelet: Sitelet<'T> -> Sitelet<obj>

    /// Reverses the Upcast operation on the sitelet.
    val UnsafeDowncast<'T when 'T : equality> :
        sitelet: Sitelet<obj> -> Sitelet<'T>

    /// Constructs a sitelet with an inferred router and a given controller
    /// function.
    val Infer<'T when 'T : equality> : ('T -> Content<'T>) -> Sitelet<'T>