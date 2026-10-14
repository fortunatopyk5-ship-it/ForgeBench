# STATUS

Current objective:
Iteration 9 audits ONLY two ingredients of the already-claimed regular R' closure:
(A) the exact polymer incompatibility behind CMP122-II (1.90);
(B) the coordinate identification between the connected cluster logarithm and Balaban's native R'^(k)(X).
No upstream Eq. (1.91), boundary B' theorem, source-mark work, K_0, or STEP_0 is attacked.

Current strongest result:
Both audit targets are now closed at the source/formula level.

A. Native compatibility:
Published CMP116 Eq. (2.11), which is the published cluster-expansion continuation used by CMP122-II's legacy references, defines the incompatibility function zeta to vanish exactly when two localization domains have an intersection containing a cube OR a full wall of a cube.

Define
Car(eta)=pi_k-cube carrier of eta,
Supp_*(eta)=closed one-step wall-neighborhood N_wall[Car(eta)].

Then
eta not~ eta'
iff
Car(eta') cap Supp_*(eta) != empty.

In d=4,
#Supp_*(eta)<=9|eta|,
so
C_inc=9.

For one incompatibility edge, the worst case is two unit 4-cubes sharing a wall. Their union has diameter sqrt(7), hence
c_stitch=sqrt(7).

Therefore for an incompatibility-connected family,
d_k(union_i eta_i)
<=
sum_i d_k(eta_i)+sqrt(7)(n-1).

The corrected native KP weight/radius are
g_theta(eta)=theta d_k(eta)+sqrt(7)theta,
q_native(rho)=exp(16a+sqrt(7)theta)rho,
r_*^native=(a/9)exp[-16a-sqrt(7)theta],
assuming 16a+theta<=mu_in.

B. Native coordinate identification:
CMP122-II says:
- the first operation is the Mayer expansion, "the same as in (7.1) [I]";
- native R'^(k)(X) is given by convergent series "(7.13) [I]" with proper notational changes.

The bibliography identifies [I] as CMP109. The published CMP109 ends before the cluster-localization construction and explicitly says it will be done in the next paper. The published continuation CMP116 Sect. 2 starts with the Mayer expansion (2.1), defines cube-or-wall compatibility in (2.11), exponentiates in (2.12), and defines the local coefficient in (2.13) by the exact-union condition union_i Z_i=X.

Thus the legacy manuscript numbering is source-crosswalked:
(7.1)[I] <-> published CMP116 (2.1),
(7.13)[I] <-> published CMP116 (2.13).

Therefore, after the proper notational replacement,
R'^(k)_Bal(X)=H_X^can(F)
coordinatewise.

This is CASE 1: EXACT, with provenance label SOURCE-CROSSWALK VERIFIED.

Revalidated theorem:
YM-RPRIME-C1-1 is again a NATIVE Balaban theorem, but with corrected constants.

Let
mu_in=(1+beta)kappa-lambda_in>0,
C_geo(lambda_in)<infinity,
16a+theta<=mu_in,

r_*=(a/9)exp[-16a-sqrt(7)theta].

Shrink the fixed small-coupling threshold gamma_R' so that
r=C_geo(lambda_in)exp[-p_0(gamma_R')]<r_*,
and set
rho=(r+r_*)/2.

Then the native regular R' coefficient map is Banach-holomorphic/C^1 and
sup_{||F||<=r}||D R'^(k)_Bal(F)||
<=
q_native(rho)/(rho-r),
q_native(rho)=exp(16a+sqrt(7)theta)rho,
uniformly in finite volume, UV cutoff and RG depth.

Quasi-local Schur:
Still conditional only on the explicit residual decay reserve
C_geo(theta-mu_out-nu)<infinity.
The corrected q_native and r_* are used; no numerical Balaban kappa threshold is invented.

Verified this iteration:
- exact native cube-or-wall incompatibility convention;
- Supp_*=closed wall-neighborhood of cube carrier;
- C_inc=9;
- c_stitch=sqrt(7);
- corrected native KP radius;
- exact-union local log formula through the legacy-to-published source crosswalk;
- coordinatewise native R'_Bal=H^can;
- revalidated native bounded C^1 theorem;
- exact derivative support eta subset X and r_col=0 remain valid.

Not attacked:
- upstream construction Eq. (1.91);
- boundary B' lane;
- full T/E/R/B history map;
- source marks;
- K_0;
- STEP_0.

Current blocker after this audit:
This run stops here as requested. The audited R' closure survives with corrected constants.
The next blocker for a future run is again upstream Eq. (1.91), but it was NOT attacked here.

Iteration-9 verdict:
STATUS: BALABAN REGULAR R' BOUNDED C^1 REVALIDATED WITH CORRECTED NATIVE COMPATIBILITY CONSTANTS.


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
