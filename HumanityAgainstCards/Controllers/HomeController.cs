using HumanityAgainstCards.Entities;
using HumanityAgainstCards.Models;
using System.Web.Mvc;

namespace HumanityAgainstCards.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateNew()
        {
            string roomCode = GameController.Instance.CreateGame();

            return RedirectToAction("play", new { room = roomCode });
        }

        [Route("Play")]
        public ActionResult Play(string room)
        {
            if (string.IsNullOrWhiteSpace(room))
            {
                return RedirectToAction("Index");
            }

            room = room.Trim().ToUpper();

            if (room.Length != 4)
            {
                return RedirectToAction("Index");
            }

            if (!GameController.Instance.DoesGameExist(room))
            {
                return RedirectToAction("gamenotfound");
            }

            PlayModel model = new PlayModel
            {
                RoomCode = room,
            };

            return View(model);
        }

        public ActionResult GameNotFound()
        {
            return View();
        }
    }
}