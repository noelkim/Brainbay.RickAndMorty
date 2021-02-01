using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Brainbay.Submission.DataAccess.Models.Enums;

namespace Brainbay.Submission.DataAccess.Models.Domain
{
    /// <summary>
    /// Domain class representing Character entity.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Prameterless constructor.
        /// Required for EF Core.
        /// </summary>
        public Character()
        {

        }

        /// <summary>
        /// Constructor of <see cref="Character"/>.
        /// </summary>
        /// <param name="id">The id of the character.</param>
        /// <param name="name">The name of the character.</param>
        /// <param name="status">The status of the character ('Alive', 'Dead' or 'unknown').</param>
        /// <param name="species">The species of the character.</param>
        /// <param name="type">The type or subspecies of the character.</param>
        /// <param name="gender">The gender of the character ('Female', 'Male', 'Genderless' or 'unknown').</param>
        /// <param name="location">Name and link to the character's last known location endpoint.</param>
        /// <param name="origin">Name and link to the character's origin location.</param>
        /// <param name="image">Link to the character's image. All images are 300x300px and most are medium shots or portraits since they are intended to be used as avatars.</param>
        /// <param name="episode">List of episodes in which this character appeared.</param>
        /// <param name="url">Link to the character's own URL endpoint.</param>
        /// <param name="created">Time at which the character was created in the database.</param>
        public Character(int id,
                         string name,
                         CharacterStatus status = CharacterStatus.Unknown,
                         string species = "",
                         string type = "",
                         CharacterGender gender = CharacterGender.Unknown,
                         Uri location = null,
                         Uri origin = null,
                         Uri image = null,
                         ICollection<string> episode = null,
                         Uri url = null,
                         DateTime? created = null)
        {
            Id = id;
            Name = name;
            Status = status;
            Species = species;
            Type = type;
            Gender = gender;
            LocationUrl = location;
            OriginUrl = origin;
            Image = image;
            Episode = episode;
            Url = url;
            Created = created;
        }

        /// <summary>
        /// Gets the id of the character.
        /// </summary>
        public int Id {get; set;}

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        [Required]
        public string Name {get; set;}

        /// <summary>
        /// Gets the status of the character ('Alive', 'Dead' or 'unknown').
        /// </summary>
        public CharacterStatus Status {get; set;}

        /// <summary>
        /// Gets the species of the character.
        /// </summary>
        public string Species {get; set;}

        /// <summary>
        /// Gets the type or subspecies of the character.
        /// </summary>
        public string Type {get; set;}

        /// <summary>
        /// Gets the gender of the character ('Female', 'Male', 'Genderless' or 'unknown').
        /// </summary>
        public CharacterGender Gender {get; set;}

        /// <summary>
        /// Gets link to the character's last known location endpoint.
        /// </summary>
        public Uri LocationUrl {get; set;}

        /// <summary>
        /// Gets link to the character's origin location.
        /// </summary>
        public Uri OriginUrl {get; set;}

        /// <summary>
        /// Gets link to the character's image. All images are 300x300px and most are medium shots or portraits since they are intended to be used as avatars.
        /// </summary>
        public Uri Image {get; set;}

        /// <summary>
        /// Gets list of episodes in which this character appeared.
        /// </summary>
        public ICollection<string> Episode {get; set;}

        /// <summary>
        /// Gets link to the character's own URL endpoint.
        /// </summary>
        public Uri Url {get; set;}

        /// <summary>
        /// Gets time at which the character was created in the database.
        /// </summary>
        public DateTime? Created {get; set;}

        public override string ToString() 
            => $"{nameof(Id)}={Id}, {nameof(Name)}={Name}, {nameof(Status)}={Status}";
    }
}
