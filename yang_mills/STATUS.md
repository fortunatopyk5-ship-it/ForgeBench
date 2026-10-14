# STATUS

Current objective:
Iteration 8 is restricted to the regular/second-group CMP122-II R' cluster/log COEFFICIENT map. K_0 and STEP_0 remain prohibited. The regular R' bounded C^1 theorem is now closed; the full YM-HISTORY-NORM-1 problem remains open upstream and in the boundary lane.

Current strongest result:
YM-RPRIME-C1-1 is PROVED for the regular R' cluster/log coefficient sector with an explicit anchored input norm and an explicit sufficient KP convergence radius. No unnamed "sufficiently small" cluster radius is used. For
mu_in=(1+beta)kappa-lambda_in>0
with lambda_in in the CMP109 geometric-summability regime, choose a,theta>0 with 16a+theta<=mu_in and
r_*=a exp[-16a-2theta].
The Balaban-generated regular pre-exponentiation activity obeys
||F^src||_in <= C_geo(lambda_in) exp[-p_0(g_k)].
Since primary CMP119 has p_0(g)->infinity as g->0, shrink the fixed source coupling threshold gamma_R' so that
r=C_geo(lambda_in)exp[-p_0(gamma_R')]<r_*,
and set rho=(r+r_*)/2.
Generalized KP gives a volume/scale-uniform Banach-holomorphic canonical cluster logarithm on the r_*-ball, and Banach Cauchy gives
sup_{||F||<=r} ||D H_R',k^reg(F)||
<= q(rho)/(rho-r),
q(rho)=exp(16a+2theta)rho,
uniformly in volume, UV cutoff and RG depth.

Run status:
REGULAR R' BOUNDED C^1 PROVED.
The stronger quasi-local Schur Jacobian is PROVED UNDER the additional explicit decay-budget condition
lambda_res=theta-mu_out-nu
lying in the CMP109 summability regime. No numerical source threshold for this extra reserve is claimed.

Verified:
- Activity-vector naming correction: the typed direct-sum object is Act_A(F), not a physical reconstruction. A future synthesis map S_A:D_A^(mu)->F_A is separate and its boundedness is UNKNOWN.
- Source/activity tangent correction: physical source space J_A and insertion map jmath_A:J_A->B_in are separate. Boundedness of jmath_A is UNKNOWN; J_A is not automatically an activity tangent.
- DIRECT PRIMARY CMP122-II (1.90)-(1.97): the pre-exponentiation R-operation output is a hard-core connected-component polymer gas with local activities F(X'); for the regular lane X' disjoint from the large-field set, c_1=exp[-p_0(g_k)] and the relative metric reduces to ordinary d_k.
- DIRECT PRIMARY CMP122-II (1.98)-(1.100): exponentiation produces connected R' activities; the second/regular output has ordinary d_k decay and amplitude exp[-p_0(g_k)].
- DIRECT PRIMARY CMP119: p_0(g)=A_0(log g^{-2})^{p_*} with positive p_*, hence p_0(gamma)->infinity as gamma->0.
- Exact regular input norm:
  ||F||_in=sup_B sum_{eta contains B} exp(mu_in d_k(eta))||F_eta||,
  with actual source input bounded by C_geo(lambda_in)exp[-p_0(g_k)].
- YM-OVERLAP-TREE-STITCH-1: for overlap-connected inputs, d_k(union eta_i)<=sum_i d_k(eta_i)+2(n-1) in d=4.
- YM-RPRIME-TREE-KP-BALL-1: explicit sufficient convergence radius r_*=a exp[-16a-2theta].
- YM-RPRIME-EXACT-UNION-1: for the canonical connected logarithm,
  D_eta H_R',gamma=0 unless eta subset gamma. The deterministic selector may be chosen inside eta, hence coefficient-map collar r_col=0.
- YM-RPRIME-UNIFORM-HOLOMORPHY-1: uniform absolute rooted-KP convergence on every rho<r_* ball gives Banach-valued holomorphy uniform in finite volume and scale.
- YM-RPRIME-C1-1: bounded C^1 into B_out(theta) with derivative constant q(rho)/(rho-r), independent of volume/cutoff/depth.
- Source-visible exponent change (1+beta)kappa -> (1+beta/2)kappa is recorded only as SOURCE-VISIBLE DECAY RESERVE beta*kappa/2; it is not identified with the actual Banach norm loss mu_in-mu_out.
- Stronger weighted Schur/quasi-local Jacobian follows if theta-mu_out-nu retains a CMP109 summability reserve.

Unverified / deliberately not attacked:
- first/boundary B' lane with modified metric d_{k,Y};
- bounded C^1 of the upstream map that constructs F(X') from preceding T/E/R/B/history data;
- full compressed outer-history map;
- bounded physical source insertion jmath_A;
- bounded physical synthesis S_A;
- any numerical source threshold guaranteeing the extra quasi-local Schur reserve;
- K_0 and STEP_0.

Current blocking lemma after the regular-sector closure:
Within the broader YM-HISTORY-NORM-1 program, the nearest blocker is now the UPSTREAM PRE-ACTIVITY MAP:
localized T/E/R/B/history data -> CMP122-II pre-exponentiation F(X').
The separate boundary B' lane is also open.
Neither is attacked in Iteration 8.

Next action:
Do not revisit regular R' cluster/log convergence. In a future run, pick exactly one of:
(A) prove bounded C^1/locality for the upstream map producing F(X'), or
(B) build the relative-metric anchored space for the boundary B' lane.
Before physical source differentiation, separately prove a bounded insertion map jmath_A.

Iteration-8 verdict:
STATUS: REGULAR R' BOUNDED C^1 PROVED; QUASI-LOCAL JACOBIAN CONDITIONAL ON AN EXPLICIT ADDITIONAL DECAY-BUDGET.


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
