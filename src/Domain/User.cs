﻿using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public Guid PublicId { get; set; }
}