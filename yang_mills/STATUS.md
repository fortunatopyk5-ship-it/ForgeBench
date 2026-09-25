# STATUS

Current objective:
Prove a cutoff/volume-uniform physical-scale clustering estimate along the asymptotically-free Wilson trajectory, or reduce it to a normalization-invariant functional inequality.

Current strongest result:
YM-BRIDGE-1: assuming an OS-reconstructible continuum limit and a common exponential Euclidean-time decay rate m_*>0 on a dense centered gauge-invariant local algebra, the reconstructed Hamiltonian obeys Spec(H)∩(0,m_*)=∅. The proof is a direct spectral-measure argument.

Current blocking lemma:
YM-IR-1 / YM-FI-1. Need a rate in lattice units gamma(a) satisfying gamma(a) >= c a Lambda_YM, uniformly in volume as a->0. A promising sufficient proxy is lambda_a/v_a >= c a Lambda_YM, where lambda_a is a reversible stochastic-quantization Poincare rate and v_a is a local commutator/influence propagation scale.

Assumptions:
- compact simple gauge group G;
- Wilson lattice regularization used for the current route;
- continuum observable renormalization/convergence is separated into YM-UV-1 rather than assumed silently.

Verified:
- finite-cutoff Wilson measure is a genuine finite-dimensional Haar Gibbs measure;
- reflection positivity / positive transfer-matrix machinery is classical for Wilson lattice gauge theory;
- strong-coupling SU(N) functional inequalities and exponential correlation decay are rigorous (Shen–R. Zhu–X. Zhu);
- clustering-to-gap bridge under OS reconstruction is proved in lemmas.md;
- positivity of a lattice gap for each fixed cutoff/volume is not sufficient for the continuum gap.

Unverified:
- full d=4 continuum OS construction on R^4;
- nontriviality of that continuum limit;
- YM-IR-1 at beta->infinity;
- YM-FI-1 at weak coupling;
- uniform thermodynamic-limit control on the continuum trajectory.

Next action:
Attack YM-FI-1 quantitatively. Derive a general covariance-decay theorem from (i) a Poincare inequality for a reversible local semigroup and (ii) a finite-propagation/commutator matrix estimate; then determine the exact scaling requirement on lambda_a/v_a and compare it with a Lambda_YM.


Update — Iteration 1b:
A new abstract bridge YM-SEMIGROUP-1 was proved:
PI relaxation + weighted finite-propagation of local gradients gives a common spatial clustering rate
gamma >= kappa lambda/[2(lambda+v_kappa)].
Thus a sufficient cutoff target is
lambda_a/(lambda_a+v_{kappa,a}) >= c a Lambda_YM.
However, adversarial analysis shows a global PI may be too strong because auxiliary slow global/topological modes need not coincide with the physical local spectrum.

A second, cleaner mass-gap target is YM-RG-LANDING-1:
prove that exact RG lands, at block scale a_*~Lambda_YM^{-1}, inside a cutoff-independent Kotecky-Preiss polymer domain. If this holds, clustering and the physical gap follow while reflection positivity is used only at the original Wilson lattice level.


Refined blocker:
YM-CROSSOVER-1 is now the sharpest proposed new lemma after the UV package.
The key observation is that all cutoff-dependent infinitely many UV scales can be handled before a FIXED matching coupling g_match. From that fixed matching surface to a fixed strong-mixing polymer domain, only a finite number M_G of RG block steps should be required if the exact nonperturbative flow can be controlled. Therefore the new mathematical target is a finite-step inclusion in an effective-action Banach space, not an analytic continuation of a bare-coupling gap formula.


## Update — Iteration 2 (YM-CROSSOVER-1 only)

STATUS:
YM-CROSSOVER-1 REDUCED TO EXPLICIT SUBLEMMAS; inclusion itself NOT proved.

Strongest new result:
The measure-only claim
R^M(K_match) subset D_KP
is insufficient for the original microscopic mass gap unless source/observable transport is controlled. The corrected crossover package is:

(A) YM-MATCH-EXTRACT-1:
extract a global, cutoff/volume-uniform K_match from the UV construction in the concrete Banach space of crossover_spec.md.

(B) YM-TUBE-1 + YM-RG-DRIFT-1:
prove finite one-step enclosures for beta and the full remainder norm. Once a uniform drift delta>0 and invariant remainder radius R exist, M_G<infinity follows and is no longer an independent heuristic assumption.

(C) YM-RG-SOURCE-1:
control source transport / conditional covariance across eliminated RG shells.

(D) Final KP landing:
Q_{alpha,mu}(z_Phi)<alpha. YM-KP-ANCHOR-1 then gives uniform cluster expansion and exponential coarse correlations.

Concrete Banach space:
B_{alpha,mu,p,rho} of globally defined gauge-invariant polymer interactions Phi_X on connected 4D block polymers, with norm
sup_B sum_{X contains B} exp(alpha|X|+mu diam(X)) ||Phi_X||_{C^p,rho}.
Full definition: crossover_spec.md.

Corrected blocker:
The hardest unresolved estimate is no longer the abstract statement "M_G finite". It is the construction of rigorous one-step Yang-Mills enclosures

F_j^-(beta,r) <= beta(R Phi) <= F_j^+(beta,r),
r(R Phi) <= G_j(beta,r)

on an invariant tube, together with source-locality estimates, starting from a rigorously extracted K_match.

Verified this iteration:
- scalar-coupling-only flow is insufficient (explicit toy counterexamples);
- q<alpha in the anchored weighted polymer norm is a sufficient KP condition;
- the finite invariant-tube induction is rigorous;
- a positive uniform beta drift plus invariant remainder tube implies cutoff-independent finite M;
- previous cutoff bridge can be strengthened to positive-time correlator convergence only (YM-CUTOFF-BRIDGE-2), so divergent equal-time renormalization constants/prefactors need not be assumed controlled.

Validated-numerics support:
scripts/verify_crossover_tube.py uses exact rational interval arithmetic to certify the finite tube induction once rigorous one-step bounds are supplied. The bundled JSON is explicitly only a toy example.


### Final refinement — Iteration 2

The one-step invariant-tube problem has now been reduced further by YM-RG-DERIV-1 and YM-RG-TAYLOR-1.

For Phi=beta W+eta with ell(eta)=0 and ||eta||<=r, define
f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||.
A rigorous one-step tube enclosure follows from five scalar bound families on each compact beta interval I_j:

1. center flow: f_j^- <= f(beta) <= f_j^+;
2. generated center remainder: h(beta) <= H_j;
3. relevant/irrelevant mixing: ||ell D R_{beta W} Q|| <= A_j;
4. irrelevant amplification: ||Q D R_{beta W} Q|| <= B_j;
5. curvature: sup_{Phi in K_j} ||D^2 R_Phi|| <= C_j.

It then suffices that

beta_{j+1}^- <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+ >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1} >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

For the exact block-integration part of R,
D E_Phi[A]=E_{Phi,V}[A] and
D^2 E_Phi[A,B]=-Cov_{Phi,V}(A,B).
Thus the fifth bound is concretely a one-shell conditional-covariance estimate, not an abstract Frechet-regularity placeholder.

Current deepest blocker:
obtain rigorous, volume-independent Yang-Mills bounds for f,H,A,B,C on at least the first intermediate-coupling interval emerging from YM-MATCH-EXTRACT-1. This is the exact next step.


### Adversarial corrections — final

1. D_KP is NOT equivalent to a mass gap. It is only a sufficient landing chart; even exactly product measures can violate a badly chosen product-Haar activity smallness condition if local weights are large. Therefore failure of YM-CROSSOVER-1 in this chart would not imply gaplessness.

2. Polymer incompatibility must be defined using the actual collar/link support of Phi_X, not just the core block set.

3. Source control must start before the finite crossover: the UV-to-matching package must deliver quasi-local source kernels for microscopic gauge-invariant observables (YM-UV-SOURCE-MATCH-1).

4. The practical finite-drift lemma is YM-RG-DRIFT-2, with a bounded terminal beta strip and a no-overshoot estimate.

Final status of Iteration 2:
STATUS: YM-CROSSOVER-1 REDUCED TO EXPLICIT LEMMAS/INEQUALITIES — NOT PROVED.

For the measure inclusion itself, the remaining Yang-Mills content is:
- MATCH: rigorous K_match extraction in the concrete global norm;
- STEP_j: rigorous f_j^±, H_j, A_j, B_j, C_j bounds for finitely many intermediate-coupling intervals;
- LAND: final Q_{alpha,mu}<alpha estimate.

For use in the mass-gap chain, additionally:
- UV-SOURCE: microscopic-to-matching source localization;
- CROSSOVER-SOURCE: shell-by-shell source/conditional-covariance bounds.

Exact next attack:
prove or falsify the FIRST one-step certificate STEP_0 in the chosen norm, starting from the strongest matching estimates that can actually be extracted from Balaban's small-field/large-field RG results.
