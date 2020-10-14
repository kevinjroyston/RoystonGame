﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityView
{
    public Guid _Id { get; set; }
    //public UnityViewOptions Options { get; set; }
    public IReadOnlyList<UnityImage> _UnityImages { get; set; }
    public TVScreenId _ScreenId { get; set; }
    public IReadOnlyList<User> _Users { get; set; }
    public IReadOnlyList<User> _VoteRevealUsers { get; set; }
    public Dictionary<string, int> _UserIdToDeltaScores { get; set; }
    public string _Title { get; set; }
    public string _Instructions { get; set; }
    public DateTime ServerTime { get; set; }
    public DateTime? _StateEndTime { get; set; }
    public UnityViewOptions _Options { get; set; }
}