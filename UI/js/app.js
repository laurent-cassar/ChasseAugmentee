/* CHASSE AU TRESOR — cursed shell */
(() => {
  const $ = (s, r = document) => r.querySelector(s);
  const $$ = (s, r = document) => [...r.querySelectorAll(s)];

  const state = {
    screen: "boot",
    progress: 0,
    stuck: false,
    selectedSpot: null,
    errors: 4,
    reality: 82,
    memeLoad: 73,
    visitors: 1337,
    found: false,
    audio: null,
  };

  const screens = $$(".screen");
  const startMenu = $("#start-menu");

  function beep(freq = 440, dur = 0.06, type = "square", gain = 0.04) {
    try {
      const ctx = state.audio || (state.audio = new (window.AudioContext || window.webkitAudioContext)());
      const o = ctx.createOscillator();
      const g = ctx.createGain();
      o.type = type;
      o.frequency.value = freq;
      g.gain.value = gain;
      o.connect(g);
      g.connect(ctx.destination);
      o.start();
      g.gain.exponentialRampToValueAtTime(0.0001, ctx.currentTime + dur);
      o.stop(ctx.currentTime + dur);
    } catch (_) { /* autoplay policies */ }
  }

  function showScreen(id) {
    state.screen = id;
    screens.forEach((el) => el.classList.toggle("active", el.dataset.screen === id));
    $$(".task-btn[data-screen]").forEach((b) => b.classList.toggle("is-active", b.dataset.screen === id));
    startMenu.classList.remove("open");
    if (id === "boot") startBoot();
    if (id === "home") bumpVisitor();
    if (id === "game") flickerGlitch();
    beep(id === "fail" ? 110 : 660, 0.08);
  }

  window.showScreen = showScreen;

  /* ----- clock ----- */
  function tickClock() {
    const d = new Date();
    let h = d.getHours();
    const m = String(d.getMinutes()).padStart(2, "0");
    const am = h >= 12 ? "PM" : "AM";
    h = h % 12 || 12;
    $("#clock").textContent = `${h}:${m} ${am}`;
  }
  tickClock();
  setInterval(tickClock, 1000);

  /* ----- start menu ----- */
  $("#start").addEventListener("click", (e) => {
    e.stopPropagation();
    startMenu.classList.toggle("open");
    beep(220, 0.05);
  });
  document.addEventListener("click", () => startMenu.classList.remove("open"));
  startMenu.addEventListener("click", (e) => e.stopPropagation());

  $$("[data-goto]").forEach((el) => {
    el.addEventListener("click", (e) => {
      e.preventDefault();
      showScreen(el.dataset.goto);
    });
  });

  document.addEventListener("keydown", (e) => {
    const map = { "1": "boot", "2": "home", "3": "game", "4": "popups", "5": "fail", "6": "win" };
    if (map[e.key]) showScreen(map[e.key]);
  });

  /* ----- boot sequence ----- */
  const bootLines = [
    { t: "Initializing Internet connection...", c: "" },
    { t: "Dialing 56k... ksshh ksshh", c: "warn" },
    { t: "Loading memes...", c: "" },
    { t: "Checking user...", c: "" },
    { t: "ERROR: user not found", c: "err" },
    { t: "Continuing anyway...", c: "ok" },
    { t: "Mounting cursed filesystem...", c: "" },
    { t: "WARNING: reality.dll is outdated", c: "warn" },
  ];

  let bootTimer = null;
  function startBoot() {
    state.progress = 0;
    state.stuck = false;
    const log = $("#boot-log");
    const fill = $("#boot-fill");
    const label = $("#boot-pct");
    log.innerHTML = "";
    fill.style.width = "0%";
    label.textContent = "0%";
    $("#err-mini").classList.add("hidden");
    if (bootTimer) clearInterval(bootTimer);

    let i = 0;
    bootTimer = setInterval(() => {
      if (i < bootLines.length) {
        const line = bootLines[i++];
        const div = document.createElement("div");
        if (line.c) div.className = line.c;
        div.textContent = "> " + line.t;
        log.appendChild(div);
        log.scrollTop = log.scrollHeight;
        beep(line.c === "err" ? 140 : 880, 0.04);
        if (line.c === "err") $("#err-mini").classList.remove("hidden");
      }
      if (state.progress < 67) state.progress += 11;
      else if (state.progress < 99) state.progress = 99;
      else if (!state.stuck) {
        state.stuck = true;
        label.textContent = "99%";
        fill.style.width = "99%";
        setTimeout(() => {
          if (state.screen !== "boot") return;
          state.progress = 100;
          fill.style.width = "100%";
          label.textContent = "100% ???";
          setTimeout(() => showScreen("home"), 700);
        }, 2200);
        return;
      }
      if (!state.stuck) {
        fill.style.width = state.progress + "%";
        label.textContent = state.progress + "%";
      }
    }, 420);
  }

  $("#btn-skip").addEventListener("click", () => {
    if (bootTimer) clearInterval(bootTimer);
    showScreen("home");
  });
  $("#btn-cancel").addEventListener("click", () => {
    beep(90, 0.2, "sawtooth", 0.06);
    $("#err-mini-text").textContent = "You cannot cancel destiny.";
    $("#err-mini").classList.remove("hidden");
  });
  $$("[data-err-dismiss]").forEach((b) => {
    b.addEventListener("click", () => $("#err-mini").classList.add("hidden"));
  });

  /* ----- homepage ----- */
  function bumpVisitor() {
    state.visitors = 1337;
    renderCounter();
    beep(1200, 0.05);
  }
  function renderCounter() {
    const s = String(state.visitors).padStart(7, "0");
    $$("#counter .d").forEach((el, i) => { el.textContent = s[i]; });
    $("#you-are").textContent = "YOU ARE VISITOR #1338";
  }

  $("#btn-start-xp").addEventListener("click", () => showScreen("game"));
  $$(".nav-stack [data-goto]").forEach((b) => { /* already bound */ });

  $("#link-robux").addEventListener("click", (e) => {
    e.preventDefault();
    spawnPopup("congrats", { x: 240, y: 160 });
  });
  $("#link-donot").addEventListener("click", (e) => {
    e.preventDefault();
    spawnPopup("error", { x: 200, y: 120 });
  });
  $("#claim-nothing").addEventListener("click", () => spawnPopup("congrats", { x: 280, y: 180 }));
  $("#nav-mystery").addEventListener("click", () => {
    flickerGlitch();
    state.reality = Math.max(12, state.reality - 7);
    updateSys();
    spawnPopup("error", { x: 220, y: 90 });
  });

  /* ----- game ----- */
  $$(".hotspot").forEach((h) => {
    h.addEventListener("click", () => {
      state.selectedSpot = h.dataset.spot;
      $$(".hotspot").forEach((x) => x.classList.remove("picked"));
      h.classList.add("picked");
      $("#dig").disabled = false;
      $("#obj-hint").textContent = "selected: " + h.dataset.spot;
      beep(520, 0.05);
    });
  });

  $("#dig").addEventListener("click", () => {
    const spot = state.selectedSpot;
    if (!spot) return;
    if (spot === "chest") {
      state.found = true;
      $("#game-prog").style.width = "100%";
      $("#game-prog-label").textContent = "1 / 1";
      beep(880, 0.12);
      setTimeout(() => showScreen("win"), 400);
    } else if (spot === "x-red") {
      state.errors += 1;
      spawnPopup("congrats", { x: 180, y: 140 });
      hurtReality(6);
    } else if (spot === "x-skull") {
      state.errors += 1;
      spawnPopup("error", { x: 260, y: 80 });
      hurtReality(10);
    } else {
      showScreen("fail");
    }
    updateSys();
  });

  function hurtReality(n) {
    state.reality = Math.max(0, state.reality - n);
    if (state.reality < 40) flickerGlitch();
    if (state.reality <= 0) showScreen("fail");
  }

  function updateSys() {
    const set = (id, v) => { const el = document.getElementById(id); if (el) el.textContent = v; };
    set("st-meme", state.memeLoad + "%");
    set("st-user", "???");
    set("st-real", state.reality + "%");
    set("st-err", String(state.errors).padStart(2, "0"));
    const bar = $("#st-real-bar");
    if (bar) bar.style.width = state.reality + "%";
    const meme = $("#st-meme-bar");
    if (meme) meme.style.width = state.memeLoad + "%";
  }

  /* ----- popups ----- */
  function spawnPopup(kind, pos) {
    const layer = $("#popup-layer");
    const tpl = kind === "congrats" ? $("#tpl-congrats") : $("#tpl-error");
    const node = tpl.content.firstElementChild.cloneNode(true);
    node.style.left = (pos?.x ?? 160 + Math.random() * 80) + "px";
    node.style.top = (pos?.y ?? 80 + Math.random() * 60) + "px";
    layer.appendChild(node);
    makeDrag(node.querySelector(".titlebar"), node);
    node.querySelectorAll("[data-close]").forEach((b) => {
      b.addEventListener("click", () => node.remove());
    });
    beep(kind === "error" ? 160 : 990, 0.1);
    flickerGlitch();
  }
  window.spawnPopup = spawnPopup;

  $("#demo-error")?.addEventListener("click", () => spawnPopup("error", { x: 80, y: 90 }));
  $("#demo-congrats")?.addEventListener("click", () => spawnPopup("congrats", { x: 360, y: 140 }));

  /* fail / win */
  $("#retry").addEventListener("click", () => {
    state.reality = 82;
    state.errors = 4;
    state.selectedSpot = null;
    $("#dig").disabled = true;
    updateSys();
    showScreen("game");
  });
  $("#giveup").addEventListener("click", () => showScreen("home"));
  $("#donotpress").addEventListener("click", () => {
    beep(40, 0.4, "sawtooth", 0.08);
    document.body.style.filter = "invert(1) hue-rotate(90deg)";
    setTimeout(() => { document.body.style.filter = ""; showScreen("boot"); }, 400);
  });
  $("#btn-continue").addEventListener("click", () => showScreen("home"));
  $("#btn-save").addEventListener("click", () => {
    $("#save-msg").textContent = "Saved to C:\\WINDOWS\\TEMP\\nobody_will_find_this.dat";
  });
  $("#btn-nobody").addEventListener("click", () => showScreen("home"));

  /* ----- drag windows ----- */
  function makeDrag(bar, win) {
    if (!bar || !win) return;
    let ox = 0, oy = 0, dragging = false;
    bar.addEventListener("mousedown", (e) => {
      if (e.target.closest(".tb-btn")) return;
      dragging = true;
      const r = win.getBoundingClientRect();
      ox = e.clientX - r.left;
      oy = e.clientY - r.top;
      win.style.position = "absolute";
      win.style.zIndex = 60;
      e.preventDefault();
    });
    window.addEventListener("mousemove", (e) => {
      if (!dragging) return;
      win.style.left = e.clientX - ox + "px";
      win.style.top = e.clientY - oy + "px";
      win.style.margin = "0";
    });
    window.addEventListener("mouseup", () => { dragging = false; });
  }
  $$(".win[data-drag]").forEach((w) => makeDrag(w.querySelector(".titlebar"), w));

  /* ----- glitch ----- */
  function flickerGlitch() {
    const o = $("#glitch-fx");
    o.classList.add("on");
    setTimeout(() => o.classList.remove("on"), 500);
  }
  setInterval(() => {
    if (Math.random() < 0.08) flickerGlitch();
    if (state.screen === "game" && Math.random() < 0.2) {
      state.memeLoad = Math.min(99, state.memeLoad + (Math.random() < 0.5 ? -1 : 1));
      updateSys();
    }
  }, 2400);

  /* ----- close / min buttons on chrome ----- */
  $$(".tb-btn.close").forEach((b) => {
    b.addEventListener("click", (e) => {
      e.stopPropagation();
      spawnPopup("error", { x: 240, y: 120 });
    });
  });

  /* ----- desk icons ----- */
  $("#icon-tresor").addEventListener("dblclick", () => showScreen("boot"));
  $("#icon-pc").addEventListener("dblclick", () => spawnPopup("error", { x: 120, y: 80 }));

  updateSys();
  showScreen("boot");
})();
