﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Brainbay.Submission.DataAccess.Models.Domain
{
    public class Location
    {
        /// <summary>
        /// Private prameterless constructor.
        /// Required for EF Core.
        /// </summary>
        private Location()
        {

        }

        /// <summary>
        /// Constructor of <see cref="Episode"/>.
        /// </summary>
        /// <param name="id">The id of the location.</param>
        /// <param name="name">The name of the location.</param>
        /// <param name="type">The type of the location.</param>
        /// <param name="dimension">The dimension in which the location is located.</param>
        /// <param name="residents">List of character who have been last seen in the location.</param>
        /// <param name="url">Link to the location's own endpoint.</param>
        /// <param name="created">Time at which the location was created in the database.</param>
        public Location(int id = 0, string name = "", string type = "", 
            string dimension = "", ICollection<string> residents = null,
            string url = null, DateTime? created = null)
        {
            Id = id;
            Name = name;
            Type = type;
            Dimension = dimension;
            Residents = residents;
            Url = url;
            Created = created;
        }

        /// <summary>
        /// Gets the id of the location.
        /// </summary>
        public int Id {get; set;}

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public string Name {get; set;}

        /// <summary>
        /// Gets the type of the location.
        /// </summary>
        public string Type {get; set;}

        /// <summary>
        /// Gets the dimension in which the location is located.
        /// </summary>
        public string Dimension {get; set;}

        /// <summary>
        /// Gets list of character who have been last seen in the location.
        /// </summary>
        public ICollection<string> Residents {get; set;}

        /// <summary>
        /// Gets link to the location's own endpoint.
        /// </summary>
        public string Url {get; set;}

        /// <summary>
        /// Gets time at which the location was created in the database. 
        /// </summary>
        public DateTime? Created {get; set;}
    }
}
