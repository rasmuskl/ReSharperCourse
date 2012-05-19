using System.Linq;
using System.Collections.Generic;
using System;

namespace Tennis
{
    public abstract class GameStateBase : IGameState
    {
        protected TennisGame Game { get; private set; }

        protected GameStateBase(TennisGame game)
        {
            Game = game;
        }

        public abstract IGameState PointAdded(Party party);

        public virtual bool GameOver
        {
            get { return false; }
        }
    }

    public abstract class MatchStateBase : IMatchState
    {
        protected TennisMatch Match { get; set; }

        protected MatchStateBase(TennisMatch match)
        {
            Match = match;
        }

        public abstract IMatchState SetAdded(Party party);
        public abstract TennisSet GetNextSet(ITournamentRules tournamentRules);

        public virtual bool MatchOver
        {
            get { return false; }
        }
    }

    public abstract class SetStateBase : ISetState
    {
        protected TennisSet Set { get; set; }

        protected SetStateBase(TennisSet set)
        {
            Set = set;
        }

        public abstract ISetState GameAdded(Party party);

        public virtual bool SetOver
        {
            get { return false; }
        }

        public abstract TennisGame GetNextGame(ITournamentRules tournamentRules);
    }

    public class TournamentAustralianOpen : ITournamentRules
    {
        // SETS:
        // 5 for male, 3 for female, 3 for doubles

        // MATCH:
        // Advantage sets / deuce sets for final sets
        // TieBreaks in other sets
        public TennisMatch StartNewMatch()
        {
            return new TennisMatch(this, GetInitialMatchState);
        }

        public IMatchState GetInitialMatchState(TennisMatch match)
        {
            return new MatchStateFiveSet(match);
        }

        public ISetState GetStandardSetState(TennisSet set)
        {
            return new SetStateTieBreakScoring(set);
        }

        public ISetState GetDecidingSetState(TennisSet set)
        {
            return new SetStateAdvantageScoring(set);
        }

        public IGameState GetStandardGameState(TennisGame game)
        {
            return new GameStateAdvantageScoring(game);
        }
    }

    public class TournamentUSOpen
    {
        // SETS:
        // 5 for male, 3 for female, 3 for doubles

        // MATCH:
        // TieBreak for all sets
        public IMatchState GetInitialMatchState(TennisMatch match)
        {
            return new MatchStateFiveSet(match);
        }

        public ISetState GetStandardSetState(TennisSet set)
        {
            return new SetStateTieBreakScoring(set);
        }

        public ISetState GetDecidingSetState(TennisSet set)
        {
            return new SetStateTieBreakScoring(set);
        }

        public IGameState GetStandardGameState(TennisGame game)
        {
            return new GameStateAdvantageScoring(game);
        }
    }

    public class TournamentDouble
    {
        // SETS:
        // 5 for male, 3 for female, 3 for doubles
        // No-ad games (no advantage 4-x wins)

        // MATCH:
        // Match-TieBreak for deciding set (to 10 instead of normal 7)
        public IMatchState GetInitialMatchState(TennisMatch match)
        {
            return new MatchStateFiveSet(match);
        }

        public ISetState GetNormalSetState(TennisSet set)
        {
            return new SetStateTieBreakScoring(set);
        }

        public ISetState GetDecidingSetState(TennisSet set)
        {
            return new SetStateTieBreakScoring(set);
        }

        public IGameState GetNormalGameState(TennisGame game)
        {
            return new GameStateSimpleNoAdScoring(game);
        }
    }

    public class GameStateAdvantage : GameStateBase
    {
        private readonly Party _advantageParty;

        public GameStateAdvantage(TennisGame game, Party advantageParty)
            : base(game)
        {
            _advantageParty = advantageParty;
        }

        public override IGameState PointAdded(Party party)
        {
            if (party == _advantageParty)
                return new GameStateWon(Game, _advantageParty);

            return new GameStateDeuce(Game);
        }
    }

    public class GameStateAdvantageScoring : GameStateBase
    {
        public GameStateAdvantageScoring(TennisGame game)
            : base(game)
        {
        }

        public override IGameState PointAdded(Party party)
        {
            if (Game.GetScore(party) == 3 && Game.GetOpposingScore(party) == 3)
                return new GameStateDeuce(Game);

            if (Game.GetScore(party) == 4)
                return new GameStateWon(Game, party);

            return this;
        }
    }

    public class SetStateAdvantageScoring : SetStateBase
    {
        public SetStateAdvantageScoring(TennisSet set)
            : base(set)
        {
        }

        public override ISetState GameAdded(Party party)
        {
            int score = Set.GetScore(party);
            int opposingScore = Set.GetOpposingScore(party);

            if (ScoreHelper.IsAboveWithMargin(score, opposingScore, 6, 2))
                return new SetStateWon(Set, party);

            return this;
        }

        public override bool SetOver
        {
            get { return false; }
        }

        public override TennisGame GetNextGame(ITournamentRules tournamentRules)
        {
            return new TennisGame(Set.PartyA, Set.PartyB, tournamentRules.GetStandardGameState);
        }
    }


    public class GameStateDeuce : GameStateBase
    {
        public GameStateDeuce(TennisGame game)
            : base(game)
        {
        }

        public override IGameState PointAdded(Party party)
        {
            return new GameStateAdvantage(Game, party);
        }
    }

    public class MatchStateFiveSet : MatchStateBase
    {
        public MatchStateFiveSet(TennisMatch match)
            : base(match)
        {
        }

        public override IMatchState SetAdded(Party party)
        {
            if (Match.GetScore(party) >= 3)
                return new MatchStateWon(Match, party);

            return this;
        }

        public override TennisSet GetNextSet(ITournamentRules tournamentRules)
        {
            Func<TennisSet, ISetState> setStateFactory = tournamentRules.GetStandardSetState;

            if (Match.ScoreA == 2 && Match.ScoreB == 2)
                setStateFactory = tournamentRules.GetDecidingSetState;

            return new TennisSet(tournamentRules, Match.PartyA, Match.PartyB, setStateFactory);
        }
    }

    public interface IGameState
    {
        IGameState PointAdded(Party party);
        bool GameOver { get; }
    }

    public interface IMatchState
    {
        IMatchState SetAdded(Party party);
        bool MatchOver { get; }
        TennisSet GetNextSet(ITournamentRules tournamentRules);
    }

    public interface ISetState
    {
        ISetState GameAdded(Party party);
        bool SetOver { get; }
        TennisGame GetNextGame(ITournamentRules tournamentRules);
    }

    public interface ITournamentRules
    {
        IMatchState GetInitialMatchState(TennisMatch match);
        ISetState GetStandardSetState(TennisSet set);
        IGameState GetStandardGameState(TennisGame game);
        ISetState GetDecidingSetState(TennisSet set);
    }

    public class Party
    {
        public Party(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public static class ScoreHelper
    {
        public static bool IsAboveWithMargin(int score, int opposingScore, int target, int margin)
        {
            if (score < target)
                return false;

            int scoreDifference = score - opposingScore;

            if (scoreDifference < margin)
                return false;

            return true;
        }
    }

    public class ScoringContext
    {
        public ScoringContext(Party partyA, Party partyB)
        {
            PartyA = partyA;
            PartyB = partyB;
        }

        public ScoringContext(Party partyA, Party partyB, int scoreA, int scoreB)
        {
            PartyA = partyA;
            PartyB = partyB;
            ScoreA = scoreA;
            ScoreB = scoreB;
        }

        public Party PartyA { get; private set; }
        public Party PartyB { get; private set; }
        public int ScoreA { get; private set; }
        public int ScoreB { get; private set; }

        public int IncreaseScore(Party party)
        {
            if (party == PartyA)
            {
                ScoreA += 1;
                return ScoreA;
            }

            ScoreB += 1;
            return ScoreB;
        }

        public int GetScore(Party party)
        {
            if (party == PartyA)
            {
                return ScoreA;
            }

            return ScoreB;
        }

        public int GetOpposingScore(Party party)
        {
            if (party == PartyB)
            {
                return ScoreA;
            }

            return ScoreB;
        }
    }

    public class GameStateSimpleNoAdScoring : GameStateBase
    {
        public GameStateSimpleNoAdScoring(TennisGame game)
            : base(game)
        {
        }

        public override IGameState PointAdded(Party party)
        {
            if (Game.GetScore(party) == 4)
                return new GameStateWon(Game, party);

            return this;
        }
    }

    public class TennisGame : ScoringContext
    {
        private IGameState _gameState;

        public TennisGame(Party partyA, Party partyB, Func<TennisGame, IGameState> gameStateFactory)
            : base(partyA, partyB)
        {
            _gameState = gameStateFactory(this);
        }

        public bool GameOver
        {
            get { return _gameState.GameOver; }
        }

        public void AddPoint(Party party)
        {
            if (GameOver)
                throw new InvalidOperationException("Cannot add points to won games.");

            IncreaseScore(party);
            _gameState = _gameState.PointAdded(party);
        }
    }

    public class TennisMatch : ScoringContext
    {
        private readonly ITournamentRules _tournamentRules;
        private IMatchState _matchState;
        private TennisSet _currentSet;

        public TennisMatch(ITournamentRules tournamentRules, Func<TennisMatch, IMatchState> matchStateFactory)
            : base(new Party("A"), new Party("B"))
        {
            _tournamentRules = tournamentRules;
            _matchState = matchStateFactory(this);
            _currentSet = _matchState.GetNextSet(tournamentRules);
        }

        public void AddPoint(Party party)
        {
            if (_matchState.MatchOver)
                throw new InvalidOperationException("Cannot add points to won matches.");

            _currentSet.AddPoint(party);

            if (_currentSet.SetOver)
            {
                IncreaseScore(party);
                _matchState = _matchState.SetAdded(party);

                if (!_matchState.MatchOver)
                    _currentSet = _matchState.GetNextSet(_tournamentRules);
            }
        }

        public int GetMatchScore(Party party)
        {
            return GetScore(party);
        }

        public int GetSetScore(Party party)
        {
            return _currentSet.GetSetScore(party);
        }
    }

    public class TennisSet : ScoringContext
    {
        private ISetState _setState;
        private readonly ITournamentRules _tournamentRules;
        private TennisGame _currentGame;

        public TennisSet(ITournamentRules tournamentRules, Party partyA, Party partyB, Func<TennisSet, ISetState> setStateFactory)
            : base(partyA, partyB)
        {
            _setState = setStateFactory(this);
            _tournamentRules = tournamentRules;
            _currentGame = _setState.GetNextGame(tournamentRules);
        }

        public void AddPoint(Party party)
        {
            if (SetOver)
                throw new InvalidOperationException("Cannot add points to won sets.");

            _currentGame.AddPoint(party);

            if (_currentGame.GameOver)
            {
                IncreaseScore(party);
                _setState = _setState.GameAdded(party);

                if (!_setState.SetOver)
                    _currentGame = _setState.GetNextGame(_tournamentRules);
            }
        }

        public bool SetOver
        {
            get { return _setState.SetOver; }
        }

        public int GetSetScore(Party party)
        {
            return GetScore(party);
        }
    }

    public class GameStateTieBreakScoring : GameStateBase
    {
        public GameStateTieBreakScoring(TennisGame game)
            : base(game)
        {
        }

        public override IGameState PointAdded(Party party)
        {
            int newScore = Game.GetScore(party);
            int oppScore = Game.GetOpposingScore(party);

            if (ScoreHelper.IsAboveWithMargin(newScore, oppScore, 7, 2))
                return new GameStateWon(Game, party);

            return this;
        }
    }

    public class SetStateTieBreakScoring : SetStateBase
    {
        public SetStateTieBreakScoring(TennisSet set)
            : base(set)
        {
        }

        public override ISetState GameAdded(Party party)
        {
            int score = Set.GetScore(party);
            int opposingScore = Set.GetOpposingScore(party);

            if (score > 6)
                return new SetStateWon(Set, party);

            if (ScoreHelper.IsAboveWithMargin(score, opposingScore, 6, 2))
                return new SetStateWon(Set, party);

            return this;
        }

        public override bool SetOver
        {
            get { return false; }
        }

        public override TennisGame GetNextGame(ITournamentRules tournamentRules)
        {
            Func<TennisGame, IGameState> gameStateFactory = tournamentRules.GetStandardGameState;

            if (Set.ScoreA == 6 && Set.ScoreB == 6)
            {
                gameStateFactory = game => new GameStateTieBreakScoring(game);
            }

            return new TennisGame(Set.PartyA, Set.PartyB, gameStateFactory);
        }
    }

    public class GameStateWon : GameStateBase
    {
        private readonly Party _winningParty;

        public Party WinningParty
        {
            get { return _winningParty; }
        }

        public GameStateWon(TennisGame game, Party winningParty)
            : base(game)
        {
            _winningParty = winningParty;
        }

        public override IGameState PointAdded(Party party)
        {
            throw new InvalidOperationException("Cannot add points to a won game.");
        }

        public override bool GameOver
        {
            get { return true; }
        }
    }

    public class MatchStateWon : MatchStateBase
    {
        private readonly Party _winningParty;

        public Party WinningParty
        {
            get { return _winningParty; }
        }

        public MatchStateWon(TennisMatch match, Party winningParty)
            : base(match)
        {
            _winningParty = winningParty;
        }

        public override IMatchState SetAdded(Party party)
        {
            throw new InvalidOperationException("Sets cannot be added to a won match.");
        }

        public override bool MatchOver
        {
            get { return true; }
        }

        public override TennisSet GetNextSet(ITournamentRules tournamentRules)
        {
            throw new InvalidOperationException("There are no more sets in this match.");
        }
    }

    public class SetStateWon : ISetState
    {
        private readonly Party _winningParty;
        private readonly TennisSet _set;

        public Party WinningParty
        {
            get { return _winningParty; }
        }

        public TennisSet Set
        {
            get { return _set; }
        }

        public SetStateWon(TennisSet set, Party winningParty)
        {
            _set = set;
            _winningParty = winningParty;
        }

        public ISetState GameAdded(Party party)
        {
            throw new InvalidOperationException("Games cannot be added to a won set.");
        }

        public bool SetOver
        {
            get { return true; }
        }

        public TennisGame GetNextGame(ITournamentRules tournamentRules)
        {
            throw new InvalidOperationException("There are no more games in this set.");
        }
    }
}